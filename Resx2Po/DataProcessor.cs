using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Resx2Po
{
    public class DataProcessor
    {
        private readonly LanguageHandler _lang;
        private readonly string _refLanguage;
        private readonly ResourcesData _resources;
        private readonly string _root;

        public DataProcessor(string root,
            string defLanguage, string refLanguage)
        {
            _root = root;
            _refLanguage = refLanguage;
            _resources = new ResourcesData();
            _lang = new LanguageHandler(defLanguage);
        }

        public void Process()
        {
            _resources.Clear();
            Process(_root, string.Empty);

            _resources.FilterByLanguage(
                _refLanguage);
            WriteResources();
        }

        private static IDictionary<string, string> GetStrings(string path)
        {
            var doc = XDocument.Load(path);
            var root = doc.Root;
            if (root == null)
                return null;

            var ns = XNamespace.Get(string.Empty);
            var result = new Dictionary<string, string>();

            var items = root
                .Elements(ns + "data")
                .Where(x => x.Attribute("type") == null)
                .ToList();

            foreach (var item in items)
            {
                var name = item.Attribute("name");
                var value = item.Element("value");

                if (name == null || value == null)
                    continue;

                var nameValue = name.Value;
                if (nameValue.StartsWith(">>"))
                    continue;

                result.Add(nameValue, value.Value);
            }

            return result;
        }

        private void Process(string path, string localPath)
        {
            var files = Directory.GetFiles(
                path, "*.resx");

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);

                var language = _lang.Detect(name);
                var local = Path.Combine(
                    localPath, _lang.GetName(name));

                var strings = GetStrings(file);
                if (strings == null || !strings.Any())
                    continue;

                _resources.Add(local,
                    new LanguageInfo(strings)
                    {
                        Language = language,
                    });
            }

            ProcessDirs(path, localPath);
        }

        private void ProcessDirs(string path, string localPath)
        {
            var directories = Directory
                .GetDirectories(path);

            foreach (var directory in directories)
            {
                var name = Path.GetFileName(directory);
                if (name == null)
                    continue;

                Process(directory, Path.Combine(
                    localPath, name));
            }
        }

        private void WriteResources()
        {
            var baseDir = Path.Combine(_root, "po");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            var languages = _resources.GetLanguages();
            foreach (var language in languages)
            {
                var strings = _resources.GetStrings(
                    language, _refLanguage);

                if (!strings.Any())
                    continue;

                var path = Path.Combine(
                    baseDir, language + ".po");

                using (var writer = new LanguageWriter(path))
                    writer.Write(strings);
            }
        }
    }
}