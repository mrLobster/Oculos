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

        private readonly string outFileName;

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
            Log.DisplayString(@" åæø?#!=¤\)\)\(\&\(\&\%\¤\&\)\¤\%\=\%\\€ \£ \$ÅØÆ_:;");
            //readDateFileAsync(inFileName);
        }

        private async void readDateFileAsync(string fileName)
        {
            string[] text = await ReadAllLinesAsync(fileName).ConfigureAwait(false);

            List<string> lines = text.ToList();

            // Lets just keep track of how many messages have been sent
            int messagesSent = 0;

            // First string of file is template file
            string templateString = lines[0];

            // Get recipients and their variables
            readRecipientAndVariables(lines);

            // As it became, I had one recipient per message, so I sent it here
            foreach(var recipient in recipients)
            {
                Message message = smsFactory.CreateMessage(templateString, recipient);
                SendMessageAsync(message);
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
                    while((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines.ToArray();
        }

        private async void SendMessageAsync(Message message)
        {
            const int fileBufferSize = 4096;

            using (var fileStream = new FileStream(outFileName, FileMode.Append, FileAccess.Write, FileShare.None, fileBufferSize))
            {
                using(StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    await sw.WriteLineAsync("Recipient : " + message.RecipientsNumber +
                                            "\tMsgCount" + message.MessageCount + "\tBody : " +
                                            "\tSize : " + message.Size + "\tBody : " + message.Body).ConfigureAwait(false);

                }
            }
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
    }
}