using System.Threading.Tasks;
using Bergfall.Oculos.Data;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class SMSFactory
    {
        //private Regex regex = new Regex(@"(?<=\{)([^}]*)(?=\})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public Message CreateMessage(Template template, Recipient recipient)
        {
            Message message = new Message();

            if (template.VariableName.Count != recipient.variables.Count)
            {
                Log.Debug("Difference in expected number of variables in template, to the number given!");
            }
            
            for (int i = 0; i < template.VariableName.Count; i++)
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

            message.Body = template.Message;
            
            return message;
        }

    }
}
