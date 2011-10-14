using System;
using System.Collections.Generic;

namespace Resx2Po
{
    public class LanguageInfo
    {
        private readonly IDictionary<string, string> _strings;

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets the strings.
        /// </summary>
        public IDictionary<string, string> Strings
        {
            get { return _strings; }
        }

        public LanguageInfo(IDictionary<string, string> strings)
        {
            if (strings == null)
                throw new ArgumentNullException("strings");

            _strings = strings;
        }
    }
}