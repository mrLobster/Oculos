using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos.Data
{
    public class Template
    {
        public List<string> VariableName { get; } = new List<string>();

        public Template(string templateString)
        {
            if(string.IsNullOrEmpty(templateString))
            {
                Log.Debug("No templatestring found!");
            }
            {
                var variableMatches = Regex.Matches(templateString, @"(?<=\{)([^}]*)(?=\})",
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                int nrOfMatches = variableMatches.Count;

                for(int i = 0; i < nrOfMatches; i++)
                {
                    var currentVariable = variableMatches[i].Value;
                    VariableName.Add(currentVariable);
                }
            }
        }
    }
}