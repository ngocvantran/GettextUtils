using System;
using System.Collections.Generic;
using System.Linq;

namespace Resx2Po
{
    public class ResourceInfo
    {
        private readonly IDictionary<string, LanguageInfo> _langs;

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        public ResourceInfo()
        {
            _langs = new Dictionary<string, LanguageInfo>();
        }

        public void Add(LanguageInfo lang)
        {
            if (lang == null)
                throw new ArgumentNullException("lang");

            _langs.Add(lang.Language, lang);
        }

        /// <summary>
        /// Gets the languages.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLanguages()
        {
            return _langs.Keys;
        }

        public IEnumerable<StringInfo> GetStrings(
            string language, string sourceLanguage)
        {
            LanguageInfo source, target;

            if (!_langs.TryGetValue(language, out target) ||
                !_langs.TryGetValue(sourceLanguage, out source))
            {
                return Enumerable.Empty<StringInfo>();
            }

            var path = Path;

            return source.Strings
                .Join(target.Strings,
                    x => x.Key, x => x.Key,
                    (s, t) => new StringInfo
                    {
                        Key = s.Key,
                        Source = s.Value,
                        Value = t.Value,
                        Path = path,
                    });
        }

        public bool HasValue(string language)
        {
            return _langs.ContainsKey(language);
        }
    }
}