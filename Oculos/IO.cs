using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bergfall.Oculos.Data;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class IO
    {
        private const int DefaultBufferSize = 4096;

        private string outFileName;

        private SMSFactory smsFactory = new SMSFactory();
        private static IO iO = null;

        private static readonly object padlock = new object();

        public static IO GetInstance
        {
            get
            {
                lock(padlock)
                {
                    return iO ?? (iO = new IO());
                }
            }
        }

        private List<string> variables = new List<string>();

        private List<Recipient> recipients = new List<Recipient>();

        private Dictionary<string, Recipient> SMStoBeSent = new Dictionary<string, Recipient>();

        private Template Template
        {
            get; set;
        }

        /// <summary>
        /// Create and return new Recipient
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private Recipient addRecipient(string phoneNumber)
        {
            if(string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentException("Phone number must be used to initialize a recipient", nameof(phoneNumber));
            }

            Recipient newRecipient = new Recipient(phoneNumber);
            recipients.Add(newRecipient);

            return newRecipient;
        }

        private IO()
        {
            var directory = Directory.GetCurrentDirectory();
            outFileName = directory + @"\out.txt";
            var inFileName = directory + @"\in.txt";

            readDateFileAsync(inFileName);
        }

        private async void readDateFileAsync(string fileName)
        {
            string[] text = await ReadAllLinesAsync(fileName);

            List<string> lines = text.ToList();

            string templateString = lines[0];

            readRecipientAndVariables(lines);
                foreach (var recipient in recipients)
                {
                    Message message = smsFactory.CreateMessage(templateString, recipient);
                    Task task = SendMessage(message);
                }
            
        }

        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        /// <summary>
        /// Reads all lines of parameter path async, returning a string[] with all the lines of the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Task<string[]> ReadAllLinesAsync(string path)
        {
            return ReadAllLinesAsync(path, Encoding.UTF8);
        }

        /// <summary>
        /// The meat of the ReadAllLines code
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static async Task<string[]> ReadAllLinesAsync(string path, Encoding encoding)
        {
            var lines = new List<string>();

            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                DefaultBufferSize, DefaultOptions))
            {
                using(StreamReader reader = new StreamReader(stream, encoding))
                {
                    string line;
                    while((line = await reader.ReadLineAsync()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines.ToArray();
        }

        private async Task SendMessage(Message message)
        {
            Task writeLineAsyncTask;
            using(StreamWriter sw = new StreamWriter(new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                writeLineAsyncTask =  sw.WriteLineAsync("message.RecipientsNumber," + message.RecipientsNumber + ", MsgCount" +
                                        message.MessageCount.ToString() + ", Body : " + message.Body);
            }
            await writeLineAsyncTask;
            return;
        }

        /// <summary>
        /// Creates Recipients and parses it's variables
        /// </summary>
        /// <param name="lines"></param>
        private void readRecipientAndVariables(List<string> lines)
        {
            try
            {
                for(int i = 1; i < lines.Count; i++)
                {
                    List<string> recipientFields = lines[i].Split(',').Select(s => s.Trim()).ToList();

                    Recipient recipient = addRecipient(recipientFields[0]);

                    for(int j = 1; j < recipientFields.Count; j++)
                    {
                        string[] keyValuePair = recipientFields[j].Split('=');
                        recipient.AddVariable(keyValuePair[0].Trim(), keyValuePair[1].Trim());
                    }
                }
            }
            catch(Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        private Template readTemplate(string templateString) => new Template(templateString);
    }
}