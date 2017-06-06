using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Bergfall.Oculos.Data;
using Microsoft.VisualBasic.CompilerServices;
using Bergfall.Oculos;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class IO
    {
        private IO iO; // only one instance of class IO is allowed

        public IO GetInstance()
        {
            return iO ?? (iO = new IO());
        }

        private List<string> variables = new List<string>();

        private string outFileName, inFileName, directory;

        private List<Recipient> recipients = new List<Recipient>();

        private Dictionary<string, Recipient> SMStoBeSent = new Dictionary<string, Recipient>();

        public Template Template
        {
            get; private set;
        }

        private Recipient addOrGetRecipient(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number must be used to initialize a recipient", nameof(phoneNumber));

            Recipient newRecipient = new Recipient(phoneNumber);
            recipients.Add(newRecipient);

            return newRecipient;
        }
        
        internal IO()
        {
            directory = Directory.GetCurrentDirectory();
            outFileName = directory + @"\out.txt";
            inFileName = directory + @"\in.txt";
            ReadDataFile(inFileName);

            GSMEncoding gsmEncoding = new GSMEncoding();
            
            
        }

        private void ReadDataFile(string fileName)
        {
            List<string> lines = File.ReadLines(fileName).ToList();

            var template = readTemplate(lines[0]);
            readRecipientAndVariables(lines);

            SMSFactory SMSFactory = new SMSFactory();
            
            foreach (var recipient in recipients)
            {
                async message = SMSFactory.CreateMessageAsync(template, recipient);
                Send(message);
            }
            
            
        }

        private void Send(async message)
        {

        }

        private void readRecipientAndVariables(List<string> lines)
        {
            for(int i = 1; i < lines.Count; i++)
            {
                var recipientFields = lines[i].Split(',').Select(s => s.Trim()).ToList();
                Recipient recipient = addOrGetRecipient(recipientFields[0]);

                for(int j = 1; j < recipientFields.Count; j++)
                {
                    string[] keyValuePair = recipientFields[j].Split('=');
                    recipient.AddVariable(keyValuePair[0].Trim(), keyValuePair[1].Trim());
                }
            }
        }
        private Template readTemplate(string templateString)
        {
            var template = new Template(templateString);
            return template;
        }
    }
}

//for(int i = 0; i<stringParts.Count; i++)
//{
//if(stringParts[i].IndexOf('{') > -1)
//{
//// If { had anything in front of it, a comma or something similar, it will now be seperated as group 1
//var parameterPartsStart = stringParts[i].Split('{');
//    if(parameterPartsStart.Length > 1)
//{
//    smsText += parameterPartsStart[0];

//    var parameterPartsEnd = parameterPartsStart[1].Split('}');

//    if(parameterPartsEnd.Length > 1)
//    {
//        smsText += parameterPartsEnd[0].ToUpper();
//        smsText += parameterPartsEnd[1];
//    }
//}
//int endOfVariable = stringParts[i].IndexOf('}');
//int variableNameLength = stringParts[i].IndexOf('}') - 2;
//smsText += stringParts[i].Substring(1, endOfVariable - 1);
//}
//else
//{
//smsText += stringParts[i];
//}
//if(i<stringParts.Count - 1)
//{
//smsText += " ";
//}
//}