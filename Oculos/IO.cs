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

        private static IO _iO;

        private static readonly object padlock = new object();

        public static IO GetInstance(string inFileName)
        {


            lock (padlock)
            {
                return _iO ?? (_iO = new IO(inFileName));
            }

        }

        /// <summary>
        /// Create and return new Recipient
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private Recipient createRecipient(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                Log.Error("Trying to create a recipient without a phonenumber!");
                return new Recipient("00000000");
            }
            else
            {
                Recipient newRecipient = new Recipient(phoneNumber);
                return newRecipient;
            }
        }
        /// <summary>
        /// Initiate with full path to file for parsing
        /// </summary>
        /// <param name="inFileName"></param>
        private IO(string inFileName)
        {
            var directory = Directory.GetCurrentDirectory();
            outFileName = directory + @"\out.txt";

            readDateFileAsync(inFileName);
        }

        private async void readDateFileAsync(string fileName)
        {
            string[] text = await ReadAllLinesAsync(fileName).ConfigureAwait(false);

            List<string> lines = text.ToList();

            // First string of file is template file
            Template template = new Template(lines[0]);

            // Get recipients and their variables
            List<Recipient> recipients = readRecipientAndVariables(lines);

            // As it became, I had one recipient per message, so I sent it here
            foreach (var recipient in recipients)
            {
                Message message = smsFactory.CreateMessage(template, recipient);
                await SendMessageAsync(message).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Creates Recipients and parses it's variables
        /// </summary>
        /// <param name="lines"></param>
        private List<Recipient> readRecipientAndVariables(List<string> lines)
        {
            var recipients = new List<Recipient>();
            try
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    List<string> recipientFields = lines[i].Split(',').Select(s => s.Trim()).ToList();

                    // Create new recipient based on phonenumber
                    Recipient recipient = createRecipient(recipientFields[1]);

                    recipients.Add(recipient);

                    for (int j = 2; j < recipientFields.Count; j += 2)
                    {
                        recipient.AddVariable(recipientFields[j], recipientFields[j + 1]);
                    }
                }
                return recipients;

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return recipients;
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
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                DefaultBufferSize, DefaultOptions))
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines.ToArray();
        }

        //private async void SendMessageAsync(Message message)
        //{


        //    using (var fileStream = new FileStream(outFileName, FileMode.Append, FileAccess.Write))
        //    {
        //        using (StreamWriter sw = new StreamWriter(fileStream))
        //        {
        //            await sw.WriteLineAsync("Recipient : " + message.RecipientsNumber +
        //                                    "\tMsgCount" + message.MessageCount + "\tBody : " +
        //                                    "\tSize : " + message.Size + "\tBody : " + message.Body).ConfigureAwait(false);

        //        }
        //    }
        //}
        private async Task SendMessageAsync(Message message)
        {
            try
            {
                string text = composeMessage(message);
                byte[] encodedText = Encoding.Unicode.GetBytes(text);

                using (FileStream sourceStream = new FileStream(outFileName,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length).ConfigureAwait(false);
                }
            }
            catch (Exception exp)
            {
                Log.Error(exp.Message);
            }
        }

        private string composeMessage(Message message)
        {
            string text = string.Format("Recipient : {0}\tMsgCount : {1}\tSize : {2} bytes\tChars : {3}\tBody : {4}{5}",
                new object[] {message.RecipientsNumber, message.MessageCount, message.SizeInBytes, message.NumberOfCharacters, message.Body, Environment.NewLine});
            return text;
        }
    } 
}