using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Po2Resx
{
    public class FileReader : IDisposable
    {
        private readonly string _lang;
        private readonly StreamReader _reader;

        public FileReader(string path)
        {
            _reader = File.OpenText(path);
            _lang = Path.GetFileNameWithoutExtension(path);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public IList<StringInfo> Read()
        {
            var strings = new List<StringInfo>();
            var pending = new List<StringInfo>();

            do
            {
                var line = _reader.ReadLine();
                if (line == null)
                    break;

                if (line.StartsWith("#:"))
                {
                    var parts = line.Substring(3)
                        .Split(':');

                    if (parts.Length != 2)
                        continue;

                    pending.Add(new StringInfo
                    {
                        Key = parts[1],
                        Path = parts[0],
                        Language = _lang,
                    });
                }
                else if (line.StartsWith("msgstr"))
                {
                    line = line.Substring(
                        8, line.Length - 9);
                    line = Unescape(line);

                    foreach (var info in pending)
                        info.Value = line;

                    strings.AddRange(pending);
                    pending.Clear();
                }
            } while (true);

            return strings;
        }

        private static string Unescape(string value)
        {
            var escapes = new Dictionary<string, string>
            {
                {"\\\\", "\\"},
                {"\\\"", "\""},
                {"\\0", "\0"},
                {"\\a", "\a"},
                {"\\b", "\b"},
                {"\\f", "\f"},
                {"\\n", "\n"},
                {"\\r", "\r"},
                {"\\t", "\t"},
                {"\\v", "\v"},
            };

            var sb = new StringBuilder(value);
            foreach (var pair in escapes)
                sb.Replace(pair.Key, pair.Value);

            return sb.ToString();
        }
    }
}