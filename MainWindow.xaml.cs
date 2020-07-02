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
        List<string[]> dataList;
        List<string> fileList;
        List<string> fullList;
        int dataElementCount;
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
                    this.IsStrictLevel.IsChecked = isChecked;
                }
            } else {
                this.txtCurrentFolder.Text = Directory.GetCurrentDirectory();
                if (File.Exists("NameID.csv")) {
                    ReadCsvData("NameID.csv");
                    this.txtCurrentCsvPath.Text = $"{this.txtCurrentFolder.Text}\\NameID.csv";
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
        private void Window_Closed(object sender, EventArgs e) {
            string iniFileContent =
                "[DefaultPath]\n" +
                $"Path = \"{CurrentWorkDirectory}\"\n\n" +
                "[DefaultCsvFile]\n" +
                $"CsvFile = \"{CurrentCsvFile}\"\n\n" +
                "[DefaultNameFormat]\n" +
                $"NameFormat = \"{CurrentNameFormat}\"\n\n" +
                "[IsStrictCheck]\n" +
                $"IsStrictCheck = \"{this.IsStrictLevel.IsChecked}\"";
            using (StreamWriter writer = new StreamWriter(defaultIniName)) {
                writer.Write(iniFileContent);
            }
        }
        private void btnCheck_Click(object sender, RoutedEventArgs e) {
            GetCheckResult();
            GetCheckInformation();
        }
        private void btnGetFiles_Click(object sender, RoutedEventArgs e) {
            GetDataList();
            GetFileList();
            GetFullList();
            Directory.CreateDirectory(defaultGetFolderName);
            string[] files = Directory.GetFiles(CurrentWorkDirectory);
            foreach (string file in files) {
                string fileName = System.IO.Path.GetFileName(file);
                if (IsContained(fileName.Split('.')[0], fullList)) {
                    string sourceFile = file;
                    string targetFile = $"{Directory.GetCurrentDirectory()}\\{defaultGetFolderName}\\{fileName}";
                    File.Copy(sourceFile, targetFile, true);
                }
            }
            GetCheckInformation();
        }
    }

    public partial class MainWindow : Window {
        private void GetDataList() {
            this.dataList = new List<string[]>();
            dataElementCount = this.txtCheckList.Text.Split('\n')[0].Split(',').Length;
            foreach (string[] data in CsvReader.ReadFromString(this.txtCheckList.Text)) {
                if (data.Length >= dataElementCount) {
                    this.dataList.Add(data);
                }
            }
        }
        private void GetFileList() {
            this.fileList = new List<string>();
            string[] fileNames = Directory.GetFiles(CurrentWorkDirectory);
            foreach (string fileName in fileNames) {
                string name = System.IO.Path.GetFileName(fileName).Split('.')[0];
                fileList.Add(name);
            }
        }
        private void GetFullList() {
            this.fullList = new List<string>();
            foreach (string[] data in dataList) {
                string possibleFileName = CurrentNameFormat;
                if (this.IsStrictLevel.IsChecked == true) {
                    possibleFileName = GetCheckName(possibleFileName, data, StrictLevel.Strict);
                } else {
                    possibleFileName = GetCheckName(possibleFileName, data, StrictLevel.Normal);
                }
                this.fullList.Add(possibleFileName);
            }
        }
        private void GetCheckResult() {
            GetDataList();
            GetFileList();
            GetFullList();
            this.txtCheckResultList.Text = "";
            Regex seeker;
            foreach (string possibleFile in this.fullList) {
                seeker = new Regex(possibleFile);
                if (!IsContained(seeker, this.fileList)) {
                    this.txtCheckResultList.Text += $"{ClearRegexPattern(possibleFile)}\n";
                }
            }
        }
        private void GetCheckInformation() {
            int a = CharCount(this.txtCheckResultList.Text, '\n');
            int b = CharCount(this.txtCheckList.Text, '\n');
            this.lblCheckInformation.Content = $"{b - a}/{b}";
        }
        private void ReadCsvData(string filePath) {
            this.txtCheckList.Text = "";
            using (FileStream csvFile = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                CsvReader csvReader = new CsvReader(csvFile);
                foreach (string[] data in csvReader.ReadRows()) {
                    this.txtCheckList.Text += $"{string.Join(",", data)}\n";
                }
            }
        }
    }
}
