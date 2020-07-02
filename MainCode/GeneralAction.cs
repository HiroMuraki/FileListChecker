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
        static public string GetCheckName(string nameFormat, string[] data, StrictLevel strictLevel = StrictLevel.Strict) {
            //正则化数据
            Regex r = new Regex("[^\\\\]\\+");
            for (int i = 0; i < data.Length; i++) {
                while (r.IsMatch(data[i])) {
                    data[i] = data[i].Replace("+", "\\+");
                }
            }
            //正则化文件名格式
            while (r.IsMatch(nameFormat)) {
                nameFormat = nameFormat.Replace("+", "\\+");
            }
            //占位符替换
            if (strictLevel == StrictLevel.Normal) {
                for (int i = 0; i < data.Length; i++) {
                    nameFormat = nameFormat.Replace($"[{i}]", $"[ -_+]*{data[i]}[ -_+]*");
                }
            } else if (strictLevel == StrictLevel.Strict) {
                for (int i = 0; i < data.Length; i++) {
                    nameFormat = nameFormat.Replace($"[{i}]", $"{data[i]}");
                }
                nameFormat = $"^{nameFormat}$";
            }
            while (nameFormat.Contains("{}")) {
                nameFormat = nameFormat.Replace("{}", "[\\s\\S]*");
            }
            return nameFormat;
        }
        static public string ClearRegexPattern(string regexString) {
            if (regexString.StartsWith("^")) {
                regexString = regexString.Replace("^", "");
            }
            if (regexString.EndsWith("$")) {
                regexString = regexString.Replace("$", "");
            }
            while (regexString.Contains("[ -_+]*")) {
                regexString = regexString.Replace("[ -_+]*", "");
            }
            while (regexString.Contains("[\\s\\S]*")) {
                regexString = regexString.Replace("[\\s\\S]*", "XXX");
            }
            while (regexString.Contains("\\+")) {
                regexString = regexString.Replace("\\+", "+");
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
        static public bool IsContained(string checkItem, List<Regex> fullList) {
            foreach (Regex seeker in fullList) {
                if (seeker.IsMatch(checkItem)) {
                    return true;
                }
            }
            return false;
        }
    }
}
