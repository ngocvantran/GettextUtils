using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Resx2Po
{
    internal class Program
    {
        private const string REGEX =
            @".+?\.([^\.]+)\.resx";

        private static void Main(string[] args)
        {
            var baseDir = Directory
                .GetCurrentDirectory();
            var baseUri = new Uri(baseDir);

            var regex = new Regex(REGEX, RegexOptions.Compiled |
                RegexOptions.Singleline);

            var ns = XNamespace.Get(string.Empty);
            var files = Directory.GetFiles(baseDir,
                "*.resx", SearchOption.AllDirectories);

            using (var writers = new Writers(baseDir))
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName == null)
                        continue;

                    var lang = "en";
                    var match = regex.Match(fileName);
                    if (match.Success)
                        lang = match.Groups[1].Value;

                    var resourcePath = "#: " + baseUri
                        .MakeRelativeUri(new Uri(file)) + ":";


                    var po = writers.Get(lang);

                    try
                    {
                        var doc = XDocument.Load(file);
                        var root = doc.Root;
                        if (root == null)
                            continue;

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

                            po.Write(resourcePath +
                                nameValue, value.Value);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
    }
}