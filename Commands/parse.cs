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
    public class Parse : ConsoleCommand
    {
        public string _date { get; set; }
        public bool _showWarnings { get; set; }
        public bool _showHelp { get; set; }
        public string _parserType { get; set; }

        public Parse()
        {
            IsCommand("parse", "Parses C&I Portal error files")
                .HasOption("d|date=", "the date to process - defaults to current date, eg: -d 20201005", x => _date = x)
                .HasOption("t|type=", "Only search File Type", x => _parserType = x)
                .HasOption("w|warings=", "Include warnings (true|FALSE) eg: -w true", x => _showWarnings = Boolean.Parse(x))
                .HasOption("h|help=", "Display Command Help eg: -h true", x => _showHelp = Boolean.Parse(x))
                .SkipsCommandSummaryBeforeRunning();

                AllowsAnyAdditionalArguments();
        }

        public override int Run(string[] remainingArguments) {
            var _status = 0;
            var _errors = 0;
            var _warnings = 0;
            ColorConsole.WriteInfo("Command => Parse");

            if (_showHelp) {
                ColorConsole.WriteInfo("Available options:");
                ColorConsole.WriteInfo("  -d, --date=VALUE      The date to parse in YYYYMMDD format eg: -d 20201001");
                ColorConsole.WriteInfo("  -t, --type=VALUE      only search File Type eg: -t ECCR");
                ColorConsole.WriteInfo("  -w, --warnings=VALUE  true or false (default is false) eg: -w true");
                ColorConsole.WriteInfo("  -h, --help            Display this command help text eg: -true");
                return 0;
            }
            
            //load parsers
            var _plugins = new Plugins()._Plugins;
            foreach (var item in _plugins)
            {
                ColorConsole.WriteSuccess($"Plugin {item.Key} loaded");
            }

            var RootPath = $"\\\\GCCSCIF01\\CI_Portal\\Error_Production";
            
            ColorConsole.WriteInfo($"Folder {RootPath}");

            if (!Directory.Exists(RootPath)) {
                ColorConsole.WriteError("ERROR=> Error_Production Folder not found.");
                ColorConsole.WriteError("Make sure you have access to the folder and try again");
                return -1;
            }

            if (_date == null) {
                _date = DateTime.Now.ToString("yyyyMMdd");
            }
            ColorConsole.WriteInfo($"Date to parse => {_date}");

            var files = Helpers.File.GetFileList(RootPath, "_error_", _date);

            // filter out files to make sure we're only looking at files on
            // the requested date
            ColorConsole.WriteLine("");

            foreach (var f in files)
            {
                if (f.ErrorDate != _date) {
                    break;
                }

                if (_parserType == null || (_parserType == f.FileType) ) {
                    // do we have a parser for this filetype?
                    var _parser = _plugins.FirstOrDefault(x => x.Key == f.FileType);
                    if (_parser == null) {
                        ColorConsole.WriteError($"No Parser definded for {f.Type} files.");
                    }
                    else {
                        var res = _parser.Run(f.Filename, !_showWarnings, null);
                        _errors += res.Errors;
                        _warnings += res.Warnings;
                        ColorConsole.WriteLine("");
                    }
                }
            }

            ColorConsole.WriteLine("");
            ColorConsole.WriteSuccess($"SUMMARY: {_errors} Errors, {_warnings} Warnings");

            return _status;
        }
    }
}