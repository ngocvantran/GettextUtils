using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Resx2Po.Properties;

namespace Resx2Po
{
    internal class Writer : IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly HashSet<string> _written;

        public Writer(string path)
        {
            _writer = new StreamWriter(path,
                false, new UTF8Encoding(false));

            _written = new HashSet<string>
            {
                string.Empty
            };

            _writer.Write(Resources.Header);
        }

        public void Dispose()
        {
            _writer.Flush();
            _writer.Dispose();
        }

        public void Write(string reference,
            string value)
        {
            if (_written.Contains(value))
                return;

            _written.Add(value);

            _writer.WriteLine();
            _writer.WriteLine();

            value = Escape(value);
            _writer.WriteLine(reference);

            _writer.WriteLine("#, c-format");

            _writer.Write("msgid \"");
            _writer.Write(value);
            _writer.WriteLine("\"");

            _writer.Write("msgstr \"");
            _writer.Write(value);
            _writer.Write("\"");
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