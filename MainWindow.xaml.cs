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

namespace NameChecker {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        #region 字段
        readonly string defaultIniName = "NameCheckerSetting.ini";
        List<string[]> dataList;
        List<string> fileList;
        List<string> fullList;
        string currentFolder;
        int dataElementCount;
        #endregion
        public MainWindow() {
            InitializeComponent();
            if (File.Exists(defaultIniName)) {
                List<string> settings = InIReader.ReadInI(defaultIniName);
                if (settings.Count >= 3) {
                    if (Directory.Exists(settings[0])) {
                        this.txtCurrentFolder.Text = settings[0];
                    } else {
                        this.txtCurrentFolder.Text = Directory.GetCurrentDirectory();
                    }
                    if (File.Exists(settings[1])) {
                        ReadCsvData(settings[1]);
                        this.txtCurrentCsvPath.Text = settings[1];
                    }
                    this.txtFileNameFormat.Text = settings[2];
                }
            } else {
                this.txtCurrentFolder.Text = Directory.GetCurrentDirectory();
                if (File.Exists("NameID.csv")) {
                    ReadCsvData("NameID.csv");
                    this.txtCurrentCsvPath.Text = $"{this.txtCurrentFolder.Text}\\NameID.csv";
                }
            }
        }
        private void btnCheck_Click(object sender, RoutedEventArgs e) {
            GetCheckResult();
            int a = CharCount(this.txtCheckResultList.Text, '\n');
            int b = CharCount(this.txtCheckList.Text, '\n');
            this.lblCheckInformation.Content = $"{b - a}/{b}";
        }
        private void btnTip_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("[0]将被替换为第一列数据，[1]将被替换为第二列数据，以此推类");
        }
        private void btnOpenFolder_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "" && fbd.SelectedPath != null) {
                this.currentFolder = fbd.SelectedPath;
                this.txtCurrentFolder.Text = currentFolder;
            }
        }
        private void btnOpenCsv_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = currentFolder;
            ofd.Filter = "csv文件|*.csv";
            ofd.ShowDialog();
            if (ofd.FileName != null && ofd.FileName != "") {
                ReadCsvData(ofd.FileName);
                this.txtCurrentCsvPath.Text = ofd.FileName;
            }
        }
        private void Window_Closed(object sender, EventArgs e) {
            string iniFileContent =
                "[DefaultPath]\n" +
                $"Path = \"{this.txtCurrentFolder.Text}\"\n\n" +
                "[DefaultCsvFile]\n" +
                $"CsvFile = \"{this.txtCurrentCsvPath.Text}\"\n\n" +
                "[DefaultNameFormat]\n" +
                $"NameFormat = \"{this.txtFileNameFormat.Text}\"";
            using (StreamWriter writer = new StreamWriter(defaultIniName)) {
                writer.Write(iniFileContent);
            }
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
            string[] fileNames = Directory.GetFiles(this.txtCurrentFolder.Text);
            foreach (string fileName in fileNames) {
                string name = System.IO.Path.GetFileName(fileName).Split('.')[0];
                fileList.Add(name);
            }
        }
        private void GetFullList() {
            this.fullList = new List<string>();
            foreach (string[] data in dataList) {
                string possibleFileName = this.txtFileNameFormat.Text;
                for (int i = 0; i < dataElementCount; i++) {
                    possibleFileName = possibleFileName.Replace($"[{i}]", data[i]);
                }
                this.fullList.Add(possibleFileName);
            }
        }
        private void GetCheckResult() {
            GetDataList();
            GetFileList();
            GetFullList();
            this.txtCheckResultList.Text = "";
            foreach (string possibleFile in this.fullList) {
                if (!IsContained(possibleFile, this.fileList)) {
                    this.txtCheckResultList.Text += $"{possibleFile}\n";
                }
            }
        }
        private bool IsContained(string item, List<string> fullList) {
            foreach (string element in fullList) {
                if (item == element) {
                    return true;
                }
            }
            return false;
        }
        private int CharCount(string str, char seekChar) {
            int i = 0;
            foreach (char ch in str) {
                if (ch == seekChar) {
                    ++i;
                }
            }
            return i;
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
