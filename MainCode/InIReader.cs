using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NameChecker {
    static class InIReader {
        static readonly Regex[] seekSetting = new Regex[4] {
            new Regex("Path[ ]*=[ ]*"),
            new Regex("CsvFile[ ]*=[ ]*"),
            new Regex("NameFormat[ ]*=[ ]*"),
            new Regex("IsStrictCheck[ ]*=[ ]*")
        };
        static public List<string> ReadInI(string filePath) {
            List<string> settings = new List<string>();
            using (StreamReader reader = new StreamReader(filePath)) {
                while (!reader.EndOfStream) {
                    string currentLine = reader.ReadLine();
                    if (currentLine.StartsWith("#")) {
                        continue;
                    }
                    foreach (Regex seeker in seekSetting) {
                        if (seeker.IsMatch(currentLine)) {
                            int startPos = seeker.Match(currentLine).Length;
                            settings.Add(currentLine.Substring(startPos).Trim('"'));
                        }
                    }
                }
            }
            return settings;
        }
    }
}
