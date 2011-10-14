using System;
using System.Collections.Generic;
using System.IO;

namespace Resx2Po
{
    internal class Writers : IDisposable
    {
        private readonly string _langsDir;
        private readonly IDictionary<string, Writer> _writers;

        public Writers(string baseDir)
        {
            _langsDir = Path.Combine(baseDir, "po");

            if (!Directory.Exists(_langsDir))
                Directory.CreateDirectory(_langsDir);

            _writers = new Dictionary<string, Writer>();
        }

        public void Dispose()
        {
            foreach (var writer in _writers.Values)
                writer.Dispose();

            _writers.Clear();
        }

        public Writer Get(string lang)
        {
            Writer writer;

            if (_writers.TryGetValue(lang, out writer))
                return writer;

            var poPath = Path.Combine(
                _langsDir, lang + ".po");

            writer = new Writer(poPath);

            _writers.Add(lang, writer);
            return writer;
        }
    }
}