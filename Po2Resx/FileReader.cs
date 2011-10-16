using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var text = string.Empty;
            var valueDetected = false;
            var strings = new List<StringInfo>();
            var pending = new List<StringInfo>();

            do
            {
                var line = _reader.ReadLine();
                
                if (valueDetected && StringUtils
                    .IsValueTerminater(line))
                {
                    if (pending.Any())
                    {
                        foreach (var info in pending)
                            info.Value = text;

                        strings.AddRange(pending);
                        pending.Clear();
                    }

                    text = string.Empty;
                    valueDetected = false;
                }

                if (line == null)
                    break;

                if (line.StartsWith("#:"))
                {
                    line = line.Substring(3);

                    var key = Path.GetFileName(line);
                    var path = Path.GetDirectoryName(line);

                    pending.Add(new StringInfo
                    {
                        Key = key,
                        Path = path,
                        Language = _lang,
                    });
                }
                else if (line.StartsWith("msgstr"))
                {
                    valueDetected = true;

                    line = line.Substring(
                        8, line.Length - 9);
                    text += StringUtils.Unescape(line);
                }
                else if (valueDetected &&
                    line.StartsWith("\""))
                {
                    line = line.Substring(
                        1, line.Length - 2);
                    text += StringUtils.Unescape(line);
                }
            } while (true);

            return strings;
        }
    }
}