using System;
using System.Collections.Generic;
using System.Text;

namespace Po2Resx
{
    internal static class StringUtils
    {
        private static readonly IDictionary<string, string> _escapes;

        static StringUtils()
        {
            _escapes = new Dictionary<string, string>
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
        }

        public static bool IsValueTerminater(string line)
        {
            if (line == null)
                return true;

            if (string.IsNullOrEmpty(line))
                return true;

            if (line.StartsWith("#:"))
                return true;

            return line.StartsWith("msgid") ||
                line.StartsWith("msgstr");
        }

        public static string Unescape(string value)
        {
            if (value == string.Empty)
                return string.Empty;

            var sb = new StringBuilder(value);
            foreach (var pair in _escapes)
                sb.Replace(pair.Key, pair.Value);

            return sb.ToString();
        }
    }
}