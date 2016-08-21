using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace GBH
{
    public class MMPFile
    {
        private Dictionary<string, Dictionary<string, string[]>> _entries = new Dictionary<string, Dictionary<string, string[]>>();

        public MMPFile(string filename)
        {
            using (StreamReader reader = File.OpenText(filename))
            {
                Read(reader);
            }
        }

        public MMPFile(StreamReader reader)
        {
            Read(reader);
        }

        public string GetValue(string section, string key)
        {
            return GetValues(section, key)[0];
        }

        public string[] GetValues(string section, string key)
        {
            string[] values;

            if (TryGetValues(section, key, out values))
            {
                return values;
            }

            throw new InvalidOperationException($"No such section/key {section}/{key}.");
        }

        public bool TryGetValue(string section, string key, out string value)
        {
            string[] values;

            if (TryGetValues(section, key, out values))
            {
                value = values[0];
                return true;
            }

            value = null;
            return false;
        }

        public bool TryGetValues(string section, string key, out string[] values)
        {
            string normalizedSection = section.ToLowerInvariant();
            string normalizedKey = key.ToLowerInvariant();

            Dictionary<string, string[]> sectionEntry;

            if (_entries.TryGetValue(normalizedSection, out sectionEntry))
            {
                if (sectionEntry.TryGetValue(normalizedKey, out values))
                {
                    return true;
                }
            }

            values = null;
            return false;
        }

        private void Read(StreamReader reader)
        {
            string sectionName = null;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (line.Length == 0)
                {
                    continue;
                }

                if (line[0] == '[')
                {
                    if (line[line.Length - 1] == ']')
                    {
                        string sectionNameRead = line.Substring(1, line.Length - 2);

                        string normalizedName = sectionNameRead.ToLowerInvariant();

                        if (_entries.ContainsKey(normalizedName))
                        {
                            throw new Exception($"Duplicate section {sectionNameRead}");
                        }

                        _entries[normalizedName] = new Dictionary<string, string[]>();

                        sectionName = normalizedName;

                        continue;
                    }
                    else
                    {
                        throw new Exception("Invalid section name (no closing ])");
                    }
                }

                if (line.Contains("="))
                {
                    if (string.IsNullOrEmpty(sectionName))
                    {
                        throw new Exception($"Key/value pair without a section ({line})");
                    }

                    string[] entry = line.Split(new[] { '=' }, 2);

                    string key = entry[0].Trim();
                    string value = entry[1].Trim().TrimStart('"').TrimEnd('"');

                    // array-ish
                    bool isArray = false;

                    if (key.EndsWith("[]"))
                    {
                        isArray = true;
                        key = key.Substring(0, key.Length - 2);
                    }

                    string normalizedKey = key.ToLowerInvariant();

                    if (_entries[sectionName].ContainsKey(normalizedKey))
                    {
                        if (!isArray)
                        {
                            throw new Exception($"Duplicate non-array key {key} in {sectionName}");
                        }
                        else
                        {
                            string[] arrayRef = _entries[sectionName][normalizedKey];
                            Array.Resize(ref arrayRef, arrayRef.Length + 1);

                            arrayRef[arrayRef.Length - 1] = value;

                            _entries[sectionName][normalizedKey] = arrayRef;
                        }
                    }
                    else
                    {
                        _entries[sectionName][normalizedKey] = new[] { value };
                    }
                }
            }
        }
    }
}
