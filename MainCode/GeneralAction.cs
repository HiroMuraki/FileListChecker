using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NameChecker.MainCode {
    class GeneralAction {
        #region 
        public enum StrictLevel {
            Normal,
            Strict
        }
        static readonly Regex seekSpecialChar = new Regex("[^\\\\][\\$\\(\\)\\*\\+\\.\\[\\]\\?\\^\\{\\}\\|]");
        static readonly Regex seekSpecialCharStart = new Regex("^[\\$\\(\\)\\*\\+\\.\\[\\]\\?\\^\\{\\}\\|]");
        static readonly Regex seekSpecialCharTrans = new Regex("[\\\\][\\$\\(\\)\\*\\+\\.\\[\\]\\?\\^\\{\\}\\|]");
        #endregion
        static public string GetCheckName(string nameFormat, string[] data, StrictLevel strictLevel = StrictLevel.Strict) {
            //正则化数据
            for (int i = 0; i < data.Length; i++) {
                data[i] = Regexilize(data[i]);
            }
            //正则化文件名格式
            nameFormat = Regexilize(nameFormat);
            //占位符替换
            while (nameFormat.Contains("\\{\\}")) {
                nameFormat = nameFormat.Replace("\\{\\}", "[\\s\\S]*");
            }
            if (strictLevel == StrictLevel.Normal) {
                for (int i = 0; i < data.Length; i++) {
                    nameFormat = nameFormat.Replace($"\\[{i}\\]", $"[ -_+]*{data[i]}[ -_+]*");
                }
            } else if (strictLevel == StrictLevel.Strict) {
                for (int i = 0; i < data.Length; i++) {
                    nameFormat = nameFormat.Replace($"\\[{i}\\]", $"{data[i]}");
                }
                nameFormat = $"^{nameFormat}$";
            }
            return nameFormat;
        }
        static public string Regexilize(string sourceString) {
            string par = seekSpecialChar.Match(sourceString).ToString();
            while (par != null && par != "") {
                sourceString = sourceString.Replace($"{par[1]}", $"\\{par[1]}");
                par = seekSpecialChar.Match(sourceString).ToString();
            }
            string parStart = seekSpecialCharStart.Match(sourceString).ToString();
            if (parStart != null && parStart != "") {
                sourceString = seekSpecialCharStart.Replace(sourceString, $"\\{parStart}");
            }
            return sourceString;
        }
        static public string ClearRegexilize(string regexString) {
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
            string par = seekSpecialCharTrans.Match(regexString).ToString();
            while (par != null && par != "") {
                regexString = regexString.Replace(par, $"{par[1]}");
                par = seekSpecialCharTrans.Match(regexString).ToString();
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
