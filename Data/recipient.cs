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

        public Dictionary<string, string> variables { get; } = new Dictionary<string, string>();
        public List<Message> Messages { get; } = new List<Message>();

        public Recipient(string phoneNumber)
        {
            TelephoneNumber = phoneNumber;
        }

        public void AddVariable(string key, string value)
        {
            if(!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value) && !variables.ContainsKey(key))
            {
                variables.Add(key, value);
            }
        }

        public string GetVariable(string templateVariable)
        {
            string variable;
            if(variables.TryGetValue(templateVariable, out variable))
            {
                return variable;
            }
            return "";
        }
    }
}