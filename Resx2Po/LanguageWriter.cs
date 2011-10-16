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

            var items = strings.ToList();

            var empties = items
                .Where(x => x.Source == string.Empty)
                .ToList();

            foreach (var empty in empties)
            {
                WriteKey(empty.Path, empty.Key);

                items.Remove(empty);
            }

            _writer.Write(Resources.Header);

            foreach (var item in items)
            {
                _writer.WriteLine();
                _writer.WriteLine();

                WriteKey(item.Path, item.Key);
                _writer.WriteLine("#, csharp-format");

                _writer.Write("msgctxt \"");
                _writer.Write(item.Context);
                _writer.WriteLine("\"");

                _writer.Write("msgid \"");
                _writer.Write(Escape(item.Source));
                _writer.WriteLine("\"");

                _writer.Write("msgstr \"");
                _writer.Write(Escape(item.Value));
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

        private void WriteKey(string path, string key)
        {
            _writer.Write("#: ");
            _writer.Write(path);
            _writer.Write(':');
            _writer.WriteLine(key);
        }
    }
}