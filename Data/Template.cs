using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos.Data
{
    public class Template
    {
        public IList<TemplateToken> TemplateTokens { get; } = new List<TemplateToken>();
        public string OriginalTemplateString {get; set; }
        public Template(string templateString)
        {
            if (string.IsNullOrEmpty(templateString))
            {
                Log.Error("Trying to make template without templatestring!");
            }
            else
            {
                OriginalTemplateString = templateString.Trim();

                // This regex uses positive lookbehind to look for starting { and captures until next } 
                // No system implemented to allow for {} in variables
                var variableMatches = Regex.Matches(templateString, @"(?<=\{)([^}]*)(?=\})",
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);


                foreach (Match currentVariable in variableMatches)
                {
                    TemplateToken templateToken = new TemplateToken() { Name = "Variable", Value = currentVariable.Value, StartIndex = currentVariable.Index, Length = currentVariable.Length };
                    TemplateTokens.Add(templateToken);
                }
            }
        }
    }
    public class TemplateToken
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
    }
}