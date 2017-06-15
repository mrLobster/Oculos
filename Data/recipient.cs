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

        public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();
        public List<Message> Messages { get; } = new List<Message>();

        public Recipient(string phoneNumber)
        {
            TelephoneNumber = phoneNumber;
        }

        public void AddVariable(string key, string value)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Trying to add variable with key null for recipient : " + TelephoneNumber);
            }
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(value, "Trying to add variable with value null for recipient : " + TelephoneNumber);
            }
            
            Variables.Add(key, value);
            
        }

        public string GetVariable(string templateVariable)
        {
            string variable;
            if(Variables.TryGetValue(templateVariable, out variable))
            {
                return variable;
            }
            return "";
        }
    }
}