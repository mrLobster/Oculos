using System;
using System.Text;
using Bergfall.Oculos.Data;
using Bergfall.Oculos.Utils;
using System.IO;

namespace Bergfall.Oculos
{
    public class SMSFactory
    {
        //private Regex regex = new Regex(@"(?<=\{)([^}]*)(?=\})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public Message CreateMessage(Template template, Recipient recipient)
        {
            Message message = new Message(recipient.TelephoneNumber);

            
                if(template.TemplateTokens.Count != recipient.Variables.Count)
                {
                    Log.Error("Difference in expected number of variables in template, to the number given!");
                    throw new ArgumentException(
                        "Difference in expected number of variables in template, to the number given!");
                }

                foreach(TemplateToken templateToken in template.TemplateTokens)
                {
                    string variable = recipient.GetVariable(templateToken.Value);
                     
                    if(string.IsNullOrEmpty(variable))
                    {
                        Log.Error("Variable : " + variable + " is not found in " +
                                  recipient.TelephoneNumber);
                        break;
                    }
                    //StringBuilder sb = new StringBuilder(template.OriginalTemplateString);
                    //sb.Append(variable, templateToken.StartIndex, templateToken.Length);
                    template.OriginalTemplateString = template.OriginalTemplateString.Replace("{" + templateToken.Value + "}", variable);
                }
            message.Body = template.OriginalTemplateString;
            
            //byte[] bytes = message.Encoding.GetBytes(message.Body);

            if(!message.Body.IsGSM())
            {
                message.Encoding = Encoding.BigEndianUnicode;
            }
            
            message.SizeInBytes = message.Encoding.GetByteCount(message.Body);
            message.MessageCount = 1 + (message.NumberOfCharacters / message.MaxNumberOfCharacters);

            return message;
        }

    }
}