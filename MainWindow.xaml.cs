using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using CsvOperate;
using Microsoft.Win32;
using static NameChecker.MainCode.GeneralAction;

namespace NameChecker {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        #region 字段
        readonly string defaultIniName = "NameCheckerSetting.ini";
        readonly string defaultGetFolderName = "提取的文件";
        readonly string defaultCsvFile = "NameID.csv";
        public string CurrentWorkDirectory {
            get {
                return this.txtCurrentFolder.Text;
            }
            set {
                this.txtCurrentFolder.Text = value;
            }
        }
        public string CurrentCsvFile {
            get {
                return this.txtCurrentCsvPath.Text;
            }
            set {
                this.txtCurrentCsvPath.Text = value;
            }
        }
        public string CurrentNameFormat {
            get {
                return this.txtFileNameFormat.Text;
            }
            set {
                this.txtFileNameFormat.Text = value;
            }
        }
        public string CheckDataList {
            get {
                return this.txtCheckList.Text;
            }
            set {
                this.txtCheckList.Text = value;
            }
        }
        public string CheckResultList {
            get {
                return this.txtCheckResultList.Text;
            }
            set {
                this.txtCheckResultList.Text = value;
            }
        }
        public bool? StrictCheck {
            get {
                return this.chkIsStrictCheck.IsChecked;
            }
            set {
                this.chkIsStrictCheck.IsChecked = value;
            }
        }
        List<string[]> dataList;
        List<string> fileList;
        List<Regex> PlanedList;
        #endregion
        public MainWindow() {
            InitializeComponent();
            if (File.Exists(defaultIniName)) {
                List<string> settings = InIReader.ReadInI(defaultIniName);
                if (settings.Count >= 4) {
                    //默认路径
                    if (Directory.Exists(settings[0])) {
                        CurrentWorkDirectory = settings[0];
                    } else {
                        CurrentWorkDirectory = Directory.GetCurrentDirectory();
                    }
                    //默认csv
                    if (File.Exists(settings[1])) {
                        ReadCsvData(settings[1]);
                        CurrentCsvFile = settings[1];
                    }
                    //默认文件名格式
                    CurrentNameFormat = settings[2];
                    //是否严格匹配
                    bool.TryParse(settings[3], out bool isChecked);
                    StrictCheck = isChecked;
                }
            } else {
                this.txtCurrentFolder.Text = Directory.GetCurrentDirectory();
                if (File.Exists(defaultCsvFile)) {
                    ReadCsvData(defaultCsvFile);
                    CurrentCsvFile = $"{CurrentWorkDirectory}\\{defaultCsvFile}";
                }
            }
        }
        private void btnOpenFolder_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "" && fbd.SelectedPath != null) {
                CurrentWorkDirectory = fbd.SelectedPath;
            }
        }
        private void btnOpenCsv_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = CurrentWorkDirectory;
            ofd.Filter = "csv文件|*.csv";
            ofd.ShowDialog();
            if (ofd.FileName != null && ofd.FileName != "") {
                ReadCsvData(ofd.FileName);
                CurrentCsvFile = ofd.FileName;
            }
        }
        private void btnCheck_Click(object sender, RoutedEventArgs e) {
            GetCheckResult();
            GetCheckInformation();
        }
        private void btnGetFiles_Click(object sender, RoutedEventArgs e) {
            GetDataList();
            GetFileList();
            GetPlanedList();
            Directory.CreateDirectory(defaultGetFolderName);
            string[] files = Directory.GetFiles(CurrentWorkDirectory);
            foreach (string file in files) {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                if (IsContained(fileName, PlanedList)) {
                    string sourceFile = file;
                    string targetFile = $"{Directory.GetCurrentDirectory()}\\{defaultGetFolderName}\\{System.IO.Path.GetFileName(file)}";
                    File.Copy(sourceFile, targetFile, true);
                }
            }
            GetCheckInformation();
        }
        private void Window_Closed(object sender, EventArgs e) {
            string iniFileContent =
                "[DefaultPath]\n" +
                $"Path = \"{CurrentWorkDirectory}\"\n\n" +
                "[DefaultCsvFile]\n" +
                $"CsvFile = \"{CurrentCsvFile}\"\n\n" +
                "[DefaultNameFormat]\n" +
                $"NameFormat = \"{CurrentNameFormat}\"\n\n" +
                "[IsStrictCheck]\n" +
                $"IsStrictCheck = \"{StrictCheck}\"";
            using (StreamWriter writer = new StreamWriter(defaultIniName)) {
                writer.Write(iniFileContent);
            }
        }
    }

    public partial class MainWindow : Window {
        private void GetPlanedList() {
            this.PlanedList = new List<Regex>();
            foreach (string[] data in dataList) {
                string possibleFileName = CurrentNameFormat;
                if (StrictCheck == true) {
                    possibleFileName = GetCheckName(possibleFileName, data, StrictLevel.Strict);
                } else {
                    possibleFileName = GetCheckName(possibleFileName, data, StrictLevel.Normal);
                }
                PlanedList.Add(new Regex(possibleFileName));
            }
        }
        private void GetDataList() {
            this.dataList = new List<string[]>();
            foreach (string[] data in CsvReader.ReadFromString(CheckDataList)) {
                this.dataList.Add(data);
            }
        }
        private void GetFileList() {
            this.fileList = new List<string>();
            string[] fileNames = Directory.GetFiles(CurrentWorkDirectory);
            foreach (string fileName in fileNames) {
                string name = System.IO.Path.GetFileNameWithoutExtension(fileName);
                fileList.Add(name);
            }
        }
        private void GetCheckResult() {
            GetDataList();
            GetFileList();
            GetPlanedList();
            CheckResultList = "";
            foreach (Regex possibleFileName in this.PlanedList) {
                if (!IsContained(possibleFileName, this.fileList)) {
                    CheckResultList += $"{ClearRegexilize(possibleFileName.ToString())}\n";
                }
            }
        }
        private void GetCheckInformation() {
            int a = CharCount(CheckResultList, '\n');
            int b = PlanedList.Count;
            this.lblCheckInformation.Content = $"{b - a}/{b}";
        }
        private void ReadCsvData(string filePath) {
            this.txtCheckList.Text = "";
            using (FileStream csvFile = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                CsvReader csvReader = new CsvReader(csvFile);
                List<string> dataList = new List<string>();
                foreach (string[] data in csvReader.ReadRows()) {
                    dataList.Add($"{string.Join(",", data)}");
                }
                CheckDataList = string.Join("\n", dataList);
            }
        }
    }
}
