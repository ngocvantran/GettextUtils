using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Resx2Po.Properties;

namespace Resx2Po
{
    public class LanguageWriter : IDisposable
    {
        private readonly StreamWriter _writer;

        public LanguageWriter(string path)
        {
            _writer = new StreamWriter(path,
                false, new UTF8Encoding(false));

            _writer.Write(Resources.Header);
        }

        public void Dispose()
        {
            _writer.Flush();
            _writer.Dispose();
        }

        public void Write(IEnumerable<StringInfo> strings)
        {
            if (strings == null)
                throw new ArgumentNullException("strings");

            var groups = strings
                .GroupBy(x => x.Source)
                .Where(x => x.Key != string.Empty);

            foreach (var group in groups)
            {
                _writer.WriteLine();
                _writer.WriteLine();

                foreach (var info in group)
                {
                    _writer.Write("#: ");
                    _writer.Write(info.Path);
                    _writer.Write(":");
                    _writer.WriteLine(info.Key);
                }

                _writer.WriteLine("#, csharp-format");

                _writer.Write("msgid \"");
                _writer.Write(Escape(group.Key));
                _writer.WriteLine("\"");

                _writer.Write("msgstr \"");
                _writer.Write(Escape(group.First().Value));
                _writer.Write("\"");
            }
        }

        private static string Escape(string value)
        {
            var escapes = new Dictionary<char, string>
            {
                {'\\', "\\\\"},
                {'\"', "\\\""},
                {'\0', "\\0"},
                {'\a', "\\a"},
                {'\b', "\\b"},
                {'\f', "\\f"},
                {'\n', "\\n"},
                {'\r', "\\r"},
                {'\t', "\\t"},
                {'\v', "\\v"},
            };

            var sb = new StringBuilder();
            foreach (var ch in value)
            {
                string replace;
                if (!escapes.TryGetValue(ch, out replace))
                    sb.Append(ch);
                else
                    sb.Append(replace);
            }

            return sb.ToString();
        }
    }
}