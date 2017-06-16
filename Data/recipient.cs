using System;
using System.Collections.Generic;

namespace Bergfall.Oculos.Data
{
    public class Recipient
    {
        public string TelephoneNumber
        {
            get; private set;
        }

        public IDictionary<string, string> Variables { get; private set; }
        //public List<Message> Messages { get; } = new List<Message>();

        public Recipient(string phoneNumber, IDictionary<string,string> variables)
        {
            TelephoneNumber = phoneNumber;
            Variables = variables;
        }

        public string GetVariable(string templateVariable)
        {
            if (Variables.TryGetValue(templateVariable, out string variable))
            {
                return variable;
            }
            return "";
        }
    }
}