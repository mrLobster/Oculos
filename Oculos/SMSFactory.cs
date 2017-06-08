using System;
using System.Text;
using Bergfall.Oculos.Data;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class SMSFactory
    {
        //private Regex regex = new Regex(@"(?<=\{)([^}]*)(?=\})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public Message CreateMessage(string templateString, Recipient recipient)
        {
            Message message = new Message();
            Template template = new Template(templateString);
            Encoding encoding = new GSMEncoding();

            int MaxNumberOfCharacters = 160;
            try
            {
                if(template.VariableName.Count != recipient.variables.Count)
                {
                    Log.Debug("Difference in expected number of variables in template, to the number given!");
                }

                for(int i = 0; i < template.VariableName.Count; i++)
                {
                    string variable = recipient.GetVariable(template.VariableName[i]);

                    if(string.IsNullOrEmpty(variable))
                    {
                        Log.Debug("Variable : " + template.VariableName[i] + " is not found in " +
                                  recipient.TelephoneNumber);
                    }

                    //Regex regex = new Regex(@"(?<=\{)" + recipient.variables.TryGetValue(template.variables[i]))
                    //([^}]*)(?=\})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    template.Message = template.Message.Replace("{" + template.VariableName[i] + "}", variable);
                }
            }
            catch(Exception exp)
            {
                Log.Debug(exp.Message);
            }

            message.Body = template.Message;

            byte[] bytes = encoding.GetBytes(template.Message);

            foreach(byte bit in bytes)
            {
                if(bit > 128)
                {
                    // Cheat by using standard Unicode if any sign is beyond the 128 limit, instead of USC2, which I think is just Unicode BigEndian?!
                    encoding = Encoding.BigEndianUnicode;
                    message.MaxNumberOfCharacters = 70;
                }
            }
            if(template.Message.Length > 160)
            {
                message.MaxNumberOfCharacters = 153;
            }
            message.MessageCount = template.Message.Length / MaxNumberOfCharacters + 1;

            return message;
        }
    }
}