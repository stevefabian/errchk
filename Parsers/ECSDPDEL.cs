using System;
using System.Collections.Generic;
using System.Composition;
using errchk.plugins;
using Gooddogs;
using Microsoft.VisualBasic.FileIO;

namespace errchk.Parsers {

    [Export(typeof(Plugin))]
    public class ECSDPDEL : Plugin {

        public override string Key => "ECSDPDEL";
        public override string Name => "ECSDPDEL Parser";
        public override Type Type => typeof(Plugin);

        private class Model {
            public int LINE { get; set; }
            public string EC_USERNAME { get; set; }
            public string SDPID { get; set; }
            public string EFD { get; set; }
            public string TYPE { get; set; }
            public string DETAILS { get; set; }
        }

        public override (int Errors, int Warnings) Run(string fullpath, bool warnings, string customerId) {

            if (customerId != null) {
                Helpers.Report.PrintTrace(fullpath, Key, customerId);
                return (0,0);
            }

            var rows = new List<Model>();
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
                        rows.Add(new Model() {
                            LINE = _line,
                            EC_USERNAME = parts[0],
                            SDPID = parts[1],
                            EFD = parts[2],
                            TYPE = parts[3],
                            DETAILS = parts[4]
                        });
                    }
                    _line++;
                }
            }

             return PrintReport(fullpath, rows, warnings);
        }

        private (int Errors, int Warnings) PrintReport(string fullpath, List<Model> rows, bool warnings) {
            var _errors = 0;
            var _warnings = 0;
            ColorConsole.WriteWarning("-----------------------------------------------------------------");
            ColorConsole.WriteInfo($"{fullpath}");
            ColorConsole.WriteInfo($"Line Count = {rows.Count}");
            ColorConsole.WriteWarning("LINE   EC_USERNAME  SDPID            DETAILS");
            ColorConsole.WriteWarning("-----------------------------------------------------------------");
            foreach (var row in rows)
            {
                if (row.DETAILS.Contains("already created")){
                    _warnings++;
                    if (warnings) {
                        ColorConsole.WriteWarning($"{row.LINE.ToString("D4")} - {row.EC_USERNAME} - {row.SDPID} - {row.DETAILS}");
                    }
                }
                else {
                    _errors++;
                    ColorConsole.WriteError($"{row.LINE.ToString("D4")} - {row.EC_USERNAME} - {row.SDPID} - {row.DETAILS}");
                }
            }
            return (_errors, _warnings);
        }

    }

}