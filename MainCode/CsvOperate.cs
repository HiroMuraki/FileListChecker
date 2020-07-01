using System;
using static System.Console;
using static System.Convert;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace CsvOperate {
    class CsvReader {
        private StreamReader reader { get; set; }
        public CsvReader(StreamReader csvStream) {
            reader = csvStream;
        }
        public CsvReader(FileStream csvStream) {
            this.reader = new StreamReader(csvStream);
        }
        public string[] ReadRow() {
            return reader.ReadLine().Split(',');
        }
        public IEnumerable<string[]> ReadRows() {
            while (!reader.EndOfStream) {
                yield return this.ReadRow();
            }
        }
        static public IEnumerable<string[]> ReadFromString(string csvString) {
            foreach (string data in csvString.Split('\n')) {
                string dataC = data.Trim('\n', '\r');
                yield return dataC.Split(',');
            }
        }
    }
    class CsvWriter {
        private StreamWriter writer { get; set; }
        public CsvWriter(StreamWriter csvStream) {
            this.writer = csvStream;
            writer.AutoFlush = true;
        }
        public CsvWriter(FileStream csvStream) {
            this.writer = new StreamWriter(csvStream);
            writer.AutoFlush = true;
        }
        public void WriteRow(string[] data) {
            writer.WriteLine(string.Join(",", data));
        }
        public void WriteRows(List<string[]> dataCollection) {
            foreach (string[] data in dataCollection) {
                this.WriteRow(data);
            }
        }
    }
}
