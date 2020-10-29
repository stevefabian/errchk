using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using errchk.Helpers;
using errchk.plugins;
using Gooddogs;
using ManyConsole;

namespace errchk.Commands
{
    public class Trace : ConsoleCommand
    {
        public string _startDate { get; set; }
        public string _endDate { get; set; }
        public string _customerId { get; set; }
        public bool _showHelp { get; set; }
        public DateTime theStartDate { get; set; }
        public DateTime theEndDate { get; set; }
        public string RootFolder { get; set; }
        public string ErrorFolder { get; set; }

        public Trace()
        {
            IsCommand("trace", "Traces a Customer ID through the transfer process")
                .HasRequiredOption("t|trace=", "Customer ID", x => _customerId = x)
                .HasOption("sd|startdate=", "the start date to process - defaults to current date, eg: -d 20201005", x => _startDate = x)
                .HasOption("ed|enddate=", "the end date to process - defaults to current date, eg: -d 20201005", x => _endDate = x)
                .HasOption("h|help=", "Display Command Help eg: -h true", x => _showHelp = Boolean.Parse(x))
                .SkipsCommandSummaryBeforeRunning();

            AllowsAnyAdditionalArguments();

            RootFolder = $"\\\\GCCSCIF01\\CI_Portal\\Mulesoft_Backup_Production";
            ErrorFolder = $"\\\\GCCSCIF01\\CI_Portal\\Error_Production";
        }

        public override int Run(string[] remainingArguments) {
            ColorConsole.WriteInfo("Command => Trace");

            ColorConsole.WriteInfo($"Folder {RootFolder}");
            if (!Directory.Exists(RootFolder)) {
                ColorConsole.WriteError("ERROR=> Mulesoft_Backup_Production Folder not found.");
                ColorConsole.WriteError("Make sure you have access to the folder and try again");
                return -1;
            }
            
            ColorConsole.WriteInfo($"Folder {ErrorFolder}");
            if (!Directory.Exists(ErrorFolder)) {
                ColorConsole.WriteError("ERROR=> Error_Production Folder not found.");
                ColorConsole.WriteError("Make sure you have access to the folder and try again");
                return -1;
            }

            var _plugins = new Plugins()._Plugins;
            foreach (var item in _plugins)
            {
                ColorConsole.WriteSuccess($"Plugin {item.Key} loaded");
            }

            if (_startDate == null) {
                _startDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else {
                _startDate = $"{_startDate.Substring(0,4)}{_startDate.Substring(4,2)}{_startDate.Substring(6,2)}";
            }

            theStartDate = DateTime.Parse($"{_startDate.Substring(4,2)}/{_startDate.Substring(6,2)}/{_startDate.Substring(0,4)}");

            if (_endDate == null) {
                _endDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else {
                _endDate = $"{_endDate.Substring(0,4)}{_endDate.Substring(4,2)}{_endDate.Substring(6,2)}";
            }

            theEndDate = DateTime.Parse($"{_endDate.Substring(4,2)}/{_endDate.Substring(6,2)}/{_endDate.Substring(0,4)}");

            ColorConsole.WriteInfo($"Start Date to parse => {theStartDate.ToString("MM/dd/yyyy")}");
            ColorConsole.WriteInfo($"End Date to parse => {theEndDate.ToString("MM/dd/yyyy")}");
            ColorConsole.WriteInfo($"Customer ID => {_customerId}");

            var files = Helpers.File.GetFileList(RootFolder, "_error_", theStartDate, theEndDate);

            ColorConsole.WriteLine("");
            ColorConsole.WriteInfo($"{files.Count()} Files Found");
           
            // DEBUG just one file
            //WriteReportLine(files.First());

            foreach (var f in files)
            {
                 // do we have a parser for this filetype?
                var _parser = _plugins.FirstOrDefault(x => x.Key == f.FileType);
                if (_parser == null) {
                    ColorConsole.WriteError($"No Parser definded for {f.Type} files.");
                }
                else {
                    var res = _parser.Run(f.Filename, true, _customerId);
                }
            }
            
            return 0;
        }
    }
}