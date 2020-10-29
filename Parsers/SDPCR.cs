using System;
using System.Collections.Generic;
using System.Composition;
using errchk.plugins;
using Gooddogs;
using Microsoft.VisualBasic.FileIO;

namespace errchk.Parsers {

    [Export(typeof(Plugin))]
    public class SDPCR : Plugin {

        public override string Key => "SDPCR";
        public override string Name => "SDPCR Parser";
        public override Type Type => typeof(Plugin);

        public override (int Errors, int Warnings) Run(string fullpath, bool warnings, string customerId) {

            if (customerId != null) {
                Helpers.Report.PrintTrace(fullpath, Key, customerId);
                return (0,0);
            }
            
            var rows = new List<Models.Row>();
            string[] parts;
            int _line = 0;
            
             using (TextFieldParser parser = new TextFieldParser(fullpath))
            {
                parser.Delimiters = new string[] { "," };
                while (true)
                {
                    parts = parser.ReadFields();
                    if (parts == null)
                    {
                        break;
                    }
                    if (_line > 0) {
                        rows.Add(new Models.Row() {
                            Line = _line,
                            ec_username = parts[0],
                            details = parts[2]
                        });
                    }
                    _line++;
                }
            }

             return PrintReport(fullpath, rows, warnings);
        }

        private (int Errors, int Warnings) PrintReport(string fullpath, List<Models.Row> rows, bool warnings) {
            var _errors = 0;
            var _warnings = 0;
            ColorConsole.WriteWarning("-----------------------------------------------------------------");
            ColorConsole.WriteInfo($"{fullpath}");
            ColorConsole.WriteInfo($"Line Count = {rows.Count}");
            ColorConsole.WriteWarning("LINE   SDPID           DETAILS");
            ColorConsole.WriteWarning("-----------------------------------------------------------------");
            foreach (var row in rows)
            {
                if (row.details.Contains("already created")){
                    _warnings++;
                    if (warnings) {
                        ColorConsole.WriteWarning($"{row.Line.ToString("D4")} - {row.ec_username} - {row.details}");
                    }
                }
                else {
                    _errors++;
                    ColorConsole.WriteError($"{row.Line.ToString("D4")} - {row.ec_username} - {row.details}");
                }
            }
            return (_errors, _warnings);
        }

    }

}