using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NameChecker.MainCode {
    class GeneralAction {
        public enum StrictLevel {
            Normal,
            Strict
        }
        static public string GetCheckName(string nameForamt, string[] data, StrictLevel strictLevel = StrictLevel.Strict) {
            if (strictLevel == StrictLevel.Normal) {
                for (int i = 0; i < data.Length; i++) {
                    nameForamt = nameForamt.Replace($"[{i}]", $"[ -_+]*{data[i]}[ -_+]*");
                }
            } else if (strictLevel == StrictLevel.Strict) {
                for (int i = 0; i < data.Length; i++) {
                    nameForamt = nameForamt.Replace($"[{i}]", $"{data[i]}");
                }
            }
            while (nameForamt.Contains("{}")) {
                nameForamt = nameForamt.Replace("{}", "[\\s\\S]*");
            }
            return nameForamt;
        }
        static public string ClearRegexPattern(string regexString) {
            while (regexString.Contains("[ -_+]*")) {
                regexString = regexString.Replace("[ -_+]*", "");
            }
            while (regexString.Contains("[\\s\\S]*")) {
                regexString = regexString.Replace("[\\s\\S]*", "XXX");
            }
            return regexString;
        }
        static public int CharCount(string str, char seekChar) {
            int i = 0;
            foreach (char ch in str) {
                if (ch == seekChar) {
                    ++i;
                }
            }
            return i;
        }
        static public bool IsContained(Regex seeker, List<string> fullList) {
            foreach (string element in fullList) {
                if (seeker.IsMatch(element)) {
                    return true;
                }
            }
            return false;
        }
        static public bool IsContained(string checkItem, List<string> fullList) {
            Regex seeker;
            foreach (string seekerPattern in fullList) {
                seeker = new Regex(seekerPattern);
                if (seeker.IsMatch(checkItem)) {
                    return true;
                }
            }
            return false;
        }
    }
}
