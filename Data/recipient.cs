using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic.CompilerServices;

namespace Bergfall.Oculos.Data
{
    public class Recipient
    {
        public string TelephoneNumber { get; } = String.Empty;
        public Dictionary<string, string> variables { get; } = new Dictionary<string, string>();
        public IList<async> Messages { get; } = new List<async>();

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