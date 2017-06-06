using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bergfall.Oculos.Data
{
    public class Template
    {
        public string Message { get; set; }
        public List<string> VariableName { get; } = new List<string>();

        public Template(string templateString)
        {
            var variableMatches = Regex.Matches(templateString, @"(?<=\{)([^}]*)(?=\})",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            int nrOfMatches = variableMatches.Count;

            for (int i = 0; i < nrOfMatches; i++)
            {
                var currentVariable = variableMatches[i].Value;
                VariableName.Add(currentVariable);
                
                Message = templateString;
            }
        }
    }
}