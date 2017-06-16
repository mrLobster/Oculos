using System;
using System.Text;
using Bergfall.Oculos.Data;
using Bergfall.Oculos.Utils;
using System.IO;
using Bergfall.Oculos.Interfaces;
using Bergfall.Oculos.Data.Interfaces;

namespace Bergfall.Oculos
{
    public class SMSFactory : IMessageSender
    {
        //private Regex regex = new Regex(@"(?<=\{)([^}]*)(?=\})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public Message CreateMessage(Template template, Recipient recipient)
        {
            Message message = new Message(recipient.TelephoneNumber);

            string text = template.OriginalTemplateString;

            // Merge variables from recipient into template
            foreach (var token in template.TemplateTokens)
            {
                var variable = recipient.GetVariable(token.Value);
                if(variable == null)
                {
                    throw new ArgumentException(String.Format("Variable : {0} is not found in {1}", token.Value, recipient.TelephoneNumber));
                }
            
               
                    //StringBuilder sb = new StringBuilder(template.OriginalTemplateString);
                    //sb.Append(variable, templateToken.StartIndex, templateToken.Length);
                text = text.Replace("{" + token.Value + "}", variable);
            }
                message.Body = text;
            
            //byte[] bytes = message.Encoding.GetBytes(message.Body);

            if(!message.Body.IsGSM())
            {
                message.Encoding = Encoding.BigEndianUnicode;
            }
            
            //message.SizeInBytes = message.Encoding.GetByteCount(message.Body);
            message.PartsCount = 1 + (message.NumberOfCharacters / message.MaxNumberOfCharacters);

            return message;
        }

        bool IMessageSender.Send(IMessage iMessage)
        {
            throw new NotImplementedException();
        }
    }
}