using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using errchk.Helpers;
using errchk.plugins;
using Gooddogs;
using ManyConsole;
using Microsoft.VisualBasic.FileIO;

namespace errchk.Commands
{
    public class Report : ConsoleCommand
    {
        public string _date { get; set; }
        public bool _showWarnings { get; set; }
        public bool _showHelp { get; set; }

        public DateTime theDate { get; set; }

        public string RootFolder { get; set; }
        public string ErrorFolder { get; set; }

        public Report()
        {
            IsCommand("report", "Create a transfer report for a specific date")
                .HasOption("d|date=", "the date to process - defaults to current date, eg: -d 20201005", x => _date = x)
                .HasOption("w|warings=", "Include warnings (true|FALSE) eg: -w true", x => _showWarnings = Boolean.Parse(x))
                .HasOption("h|help=", "Display Command Help eg: -h true", x => _showHelp = Boolean.Parse(x))
                .SkipsCommandSummaryBeforeRunning();

                AllowsAnyAdditionalArguments();

            RootFolder = $"\\\\GCCSCIF01\\CI_Portal\\Mulesoft_Backup_Production";
            ErrorFolder = $"\\\\GCCSCIF01\\CI_Portal\\Error_Production";
        }

        public override int Run(string[] remainingArguments) {
            ColorConsole.WriteInfo("Command => Report");

            ColorConsole.WriteInfo($"Folder {RootFolder}");
            if (!Directory.Exists(RootFolder)) {
                ColorConsole.WriteError("ERROR=> Error_Production Folder not found.");
                ColorConsole.WriteError("Make sure you have access to the folder and try again");
                return -1;
            }

            if (_date == null) {
                _date = DateTime.Now.ToString("yyyyMMdd");
            }
            else {
                _date = $"{_date.Substring(0,4)}{_date.Substring(4,2)}{_date.Substring(6,2)}";
            }

            theDate = DateTime.Parse($"{_date.Substring(4,2)}/{_date.Substring(6,2)}/{_date.Substring(0,4)}");

            ColorConsole.WriteInfo($"Date to parse => {theDate.ToString("MM/dd/yyyy")}");
            var directory = new DirectoryInfo(RootFolder);
            var files = directory.GetFiles().Where(file => file.CreationTime.Date == theDate);

            ColorConsole.WriteLine("");
            ColorConsole.WriteInfo($"{files.Count()} Files Found");
            WriteReportHeader();

            // DEBUG just one file
            //WriteReportLine(files.First());

            foreach (var f in files)
            {
                WriteReportLine(f);
            }

            return 0;
        }

        private void WriteReportHeader() {
            ColorConsole.WriteLine("+------------------------------------------------------------------------------------------+");
            ColorConsole.WriteLine("| FILENAME                                                                                 |");
            ColorConsole.WriteLine("| FILETYPE     |  MULESOFT |  MULESOFT | MESSAGEWAY | MESSAGEWAY |  UPLIGHT  |   UPLIGHT   |");
            ColorConsole.WriteLine("| ECCR         |    IN     |    OUT    |     IN     |    OUT     |    ACK    |     ERR     |");
            ColorConsole.WriteLine("+------------------------------------------------------------------------------------------+");
        }

        private void WriteReportLine(FileInfo f) {
            var _name = f.FullName.Substring(f.FullName.LastIndexOf("\\") + 1);
            var parts = _name.Split(new char[] { '_' });

            string _mulesoft_in = "YES";
            string _mulesoft_out = FindMulesoftOut(_name);
            string _messageway_in = FindMessageWayIn(_name);
            string _messageway_out = FindMessageWayOut(_name);
            Tuple<string, string> _uplight_ack = FindUplightAckErr(_name);

            ColorConsole.WriteLine($"| {_name.PadRight(88, ' ')} |", ConsoleColor.White);
            ColorConsole.Write($"| {parts[1].PadRight(12, ' ')} ");
            WriteEntry(_mulesoft_in, 8);
            WriteEntry(_mulesoft_out, 8);
            WriteEntry(_messageway_in, 9);
            WriteEntry(_messageway_out, 9);
            WriteEntry(_uplight_ack.Item1, 8, ConsoleColor.Green);
            WriteEntry(_uplight_ack.Item2, 10, ConsoleColor.Red);
            ColorConsole.WriteLine("|", ConsoleColor.White);
            ColorConsole.WriteLine($"+------------------------------------------------------------------------------------------|", ConsoleColor.White);
        }

        private string WriteEntry(string v, int l, ConsoleColor? c = ConsoleColor.Yellow) {
            var _c = c;

            if (v.ToLower() == "yes") {
                _c = ConsoleColor.Green;
            }

            if (v.ToLower() == "no") {
                _c = ConsoleColor.Red;
            }

            ColorConsole.Write($"|   {v.PadRight(l, ' ')}", _c);
            return "";
        }

        private string FindMulesoftOut(string name) {
            return "---";
        }

        private string FindMessageWayIn(string name) {
            return "---";
        }

        private string FindMessageWayOut(string name) {
            return "---";
        }

        private Tuple<string, string> FindUplightAckErr(string name) {
            var _parts = name.Split(new char[] {'_'} );
            var _dateStr = theDate.ToString("yyyyMMdd");
            var _filename = $"{_parts[0]}_{_parts[1]}_{_dateStr}";

            
            var directory = new DirectoryInfo(ErrorFolder);
            var _datePattern = $"_{theDate.ToString("yyyyMMdd")}-";
            var files = directory.GetFiles().Where(file => file.FullName.Contains(_filename)
                && file.FullName.Contains("_ack_"));

            if (files.Count() == 0) {
                return new Tuple<string, string>("---", "---");
            }
            else {
                using (TextFieldParser parser = new TextFieldParser(files.First().FullName)) {
                    parser.Delimiters = new string[] { "," };
                    // header  
                    var header = parser.ReadFields();
                    var fields = parser.ReadFields();

                    return new Tuple<string, string>(fields[3], fields[4]);
                };
            }
        }
    }
}