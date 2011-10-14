using System;
using System.Collections.Generic;
using System.IO;

namespace Po2Resx
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var baseDir = Directory.GetCurrentDirectory();

            var langDir = Path.Combine(baseDir, "po");
            if (!Directory.Exists(langDir))
                return;

            var files = Directory
                .GetFiles(langDir, "*.po");

            var strings = new List<StringInfo>();

            foreach (var file in files)
            {
                using (var reader = new FileReader(file))
                    strings.AddRange(reader.Read());
            }

            new ResourcesManager(baseDir, "en")
                .Write(strings);
        }
    }
}