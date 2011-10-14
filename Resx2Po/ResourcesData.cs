using System;
using System.Collections.Generic;
using System.Linq;

namespace Resx2Po
{
    public class ResourcesData
    {
        private readonly IDictionary<string, ResourceInfo> _resources;

        public ResourcesData()
        {
            _resources = new Dictionary<string, ResourceInfo>();
        }

        public void Add(string name, LanguageInfo lang)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (lang == null) throw new ArgumentNullException("lang");

            ResourceInfo resource;
            if (!_resources.TryGetValue(name, out resource))
            {
                resource = new ResourceInfo
                {
                    Path = name,
                };

                _resources.Add(name, resource);
            }

            resource.Add(lang);
        }

        public void Clear()
        {
            _resources.Clear();
        }

        /// <summary>
        /// Remove any resource that doesn't have
        /// the specified language.
        /// </summary>
        /// <param name="language">The language.</param>
        public void FilterByLanguage(string language)
        {
            var removes = _resources
                .Where(x => !x.Value
                    .HasValue(language))
                .ToList();

            foreach (var remove in removes)
                _resources.Remove(remove);
        }

        /// <summary>
        /// Gets the languages.
        /// </summary>
        /// <returns></returns>
        public string[] GetLanguages()
        {
            return _resources.Values
                .SelectMany(x => x.GetLanguages())
                .Distinct()
                .ToArray();
        }

        public IList<StringInfo> GetStrings(
            string language, string sourceLanguage)
        {
            return _resources.Values.SelectMany(
                x => x.GetStrings(language, sourceLanguage))
                .ToList();
        }
    }
}