using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bergfall.Oculos.Data;
using Bergfall.Oculos.Utils;
using Bergfall.Oculos.Interfaces;

namespace Bergfall.Oculos
{
    public class Domain
    {
        //private static readonly Lazy<Domain> lazy = new Lazy<Domain>(() => new Domain());
        
        public IList<Recipient> Recipients;
        private readonly string outFileName;
        private Template template;
        private IO iO = new IO();

        private SMSFactory smsFactory = new SMSFactory();
       
        //public static Domain Instance
        //{
        //    get { return lazy.Value; }
        //}
        /// <summary>
        /// Initiate with full path to file for parsing
        /// </summary>
        internal Domain()
        {
            var directory = Directory.GetCurrentDirectory();
            outFileName = directory + @"\out.txt";    
        }
        public void ParseInputFile(string fileName)
        {
            IList<string> lines = iO.ReadFileToList(fileName);
            
            // First string of file is template file
            template = new Template(lines[0]);

            // Get recipients and their variables
            Recipients = processInputLines(lines).ToList();
        }
        public async Task SendMessages(IMessageSender iMessageSender)
        {
            // one recipient per message
            foreach (var recipient in Recipients)
            {
                Message message = smsFactory.CreateMessage(template, recipient);
                await SendMessageAsync(message).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates Recipients and parses it's variables
        /// </summary>
        /// <param name="lines"></param>
        private IEnumerable<Recipient> processInputLines(IList<string> lines)
        {
            var recipients = new List<Recipient>();
            
            try
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    List<string> recipientFields = lines[i].Split(',').Select(s => s.Trim()).ToList();

                    var variables = new Dictionary<string, string>();

                    for (int j = 2; j < recipientFields.Count; j += 2)
                    {
                        variables.Add(recipientFields[j], recipientFields[j + 1]);
                    }
                    // Create new recipient based on phonenumber
                    Recipient recipient = new Recipient(recipientFields[1], variables);

                    recipients.Add(recipient);
                }
                return recipients;

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return recipients;
        }

        private async Task SendMessageAsync(Message message)
        {
            string text = composeRecord(message);

            byte[] encodedText = message.Encoding.GetBytes(text);
            
            using (FileStream sourceStream = new FileStream(outFileName, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length).ConfigureAwait(false);
            }

            Log.WriteLine(composeRecord(message), @"e:\code\Oculos\Oculos\log.txt");
        }
    
    /// <summary>
    /// Formats a summary of SMS transaction to string
    /// </summary>
    /// <param name="message">A fully clothed Message objec</param>
    /// <returns></returns>
    private string composeRecord(Message message)
    {
        string text = string.Format("Recipient : {0}\tMsgCount : {1}\tSize : {2} bytes\tChars : {3}\tBody : {4}{5}",
            new object[] { message.RecipientsNumber, message.PartsCount, message.SizeInBytes, message.NumberOfCharacters, message.Body, Environment.NewLine });
        return text;
    }
}
}