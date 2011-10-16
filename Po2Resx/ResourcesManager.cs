using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

namespace Po2Resx
{
    public class ResourcesManager
    {
        private readonly string _defLangauge;
        private readonly string _root;

        public ResourcesManager(string root, string defLangauge)
        {
            _root = root;
            _defLangauge = defLangauge;
        }

        public void Write(IList<StringInfo> strings)
        {
            var resources = strings
                .GroupBy(x => new StringGroup(x))
                .Select(x => new ResourceInfo
                {
                    Group = x.Key,
                    Path = GetFilePath(x.Key),
                    Strings = x.ToDictionary(
                        y => y.Key, y => y.Value)
                })
                .ToList();

            foreach (var resource in resources)
            {
                if (File.Exists(resource.Path))
                {
                    Update(resource);
                    continue;
                }

                Create(resource);
            }
        }

        private static void Create(ResourceInfo resource)
        {
            var path = resource.Path;

            using (var fs = File.Create(path))
            using (var writer = new ResXResourceWriter(fs))
            {
                foreach (var item in resource.Strings)
                {
                    writer.AddResource(
                        item.Key, item.Value);
                }
            }

            Console.WriteLine("Created new resource {0}!", path);
        }

        private string GetFilePath(StringGroup group)
        {
            var path = Path.Combine(
                _root, group.Path);

            if (!IsDefault(group))
                path += "." + group.Language;

            return path + ".resx";
        }

        private bool IsDefault(StringGroup group)
        {
            return group.Language == _defLangauge;
        }

        private void Update(ResourceInfo resource)
        {
            var hasChanges = false;
            var path = resource.Path;
            var group = resource.Group;

            var doc = XDocument.Load(path);
            var ns = XNamespace.Get(string.Empty);
            var nsXml = XNamespace.Get("http://www.w3.org/XML/1998/namespace");

            var root = doc.Root;
            if (root == null)
            {
                throw new IOException(
                    "Invalid resource file: " + path);
            }

            var existing = root
                .Elements(ns + "data")
                .Where(x => x.Attribute("type") == null)
                .Select(x => new
                {
                    Element = x,
                    Name = x.Attribute("name"),
                })
                .Where(x => x.Name != null)
                .Select(x => new
                {
                    x.Element,
                    Name = x.Name.Value,
                })
                .Where(x => !x.Name.StartsWith(">>"))
                .ToDictionary(x => x.Name, x => x.Element);

            if (!IsDefault(group))
            {
                /* Not default language
                 * Remove untranslated keys */

                var removes = existing.Keys
                    .Except(resource.Strings.Keys)
                    .ToList();

                foreach (var remove in removes)
                {
                    hasChanges = true;
                    existing[remove].Remove();
                    existing.Remove(remove);
                }
            }

            foreach (var item in resource.Strings)
            {
                XElement element;
                if (existing.TryGetValue(item.Key, out element))
                {
                    var valueElement = element.Element(ns + "value");
                    if (valueElement != null && valueElement.Value != item.Value)
                    {
                        hasChanges = true;
                        valueElement.Value = item.Value;
                    }
                }
                else
                {
                    hasChanges = true;
                    root.Add(new XElement(ns + "data",
                        new XAttribute("name", item.Key),
                        new XAttribute(nsXml + "space", "preserve"),
                        new XElement(ns + "value", item.Value)));
                }
            }

            if (!hasChanges)
                return;

            doc.Save(path);
            Console.WriteLine("Updated {0}!", path);
        }

        private class ResourceInfo
        {
            public StringGroup Group { get; set; }
            public string Path { get; set; }
            public Dictionary<string, string> Strings { get; set; }
        }
    }
}