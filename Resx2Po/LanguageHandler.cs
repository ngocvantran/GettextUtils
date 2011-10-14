using System;
using System.Text.RegularExpressions;

namespace Resx2Po
{
    public class LanguageHandler
    {
        private readonly string _defLanguage;
        private readonly Regex _regex;

        public LanguageHandler(string defLanguage)
        {
            _defLanguage = defLanguage;
            _regex = new Regex(@"\.([^.]+)$",
                RegexOptions.Compiled | RegexOptions.Singleline);
        }

        /// <summary>
        /// Detects the language name from
        /// filename (without extension).
        /// </summary>
        /// <param name="name">The filename.</param>
        /// <returns>Language name.</returns>
        public string Detect(string name)
        {
            var match = _regex.Match(name);
            return !match.Success
                ? _defLanguage
                : match.Groups[1].Value;
        }

        public string GetName(string name)
        {
            var match = _regex.Match(name);
            return !match.Success ? name
                : name.Remove(match.Index);
        }
    }
}