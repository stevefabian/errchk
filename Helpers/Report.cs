using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using errchk.Models;
using errchk.plugins;
using Gooddogs;
using Microsoft.VisualBasic.FileIO;

namespace errchk.Helpers {

    public static class Report {

        public static void PrintTrace(string path, string type, string customerId) {
            var rows = new List<Models.Row>();
            string[] parts, h;
            int _line = 0;
            h = new string[0];

            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.Delimiters = new string[] { "\t" };
                while (true)
                {
                    parts = parser.ReadFields();
                    if (parts == null)
                    {
                        break;
                    }

                    if (_line == 0) {
                        // save header
                        h = parts;
                    }

                    if (_line > 0) {

                        if(LineHasCustomerId(parts, customerId)) {
                            if (rows.Count == 0) {
                                // this is the first found row, so write the header
                                ColorConsole.WriteInfo($"{path}");
                                Helpers.Report.PrintHeader(h);
                            }

                            rows.Add(new Models.Row() {
                                Line = _line,
                                sdp_id = parts[0]
                            });

                            Helpers.Report.PrintRow(parts);
                        }
                    }
                    _line++;
                }
            }

            // if (rows.Count == 0){
            //     ColorConsole.WriteInfo($"{type} => No reference for customer found");
            // }
        }

        private static bool LineHasCustomerId(string[] p, string c) {
            var _found = false;

            foreach (var item in p)
            {
                if (item.ToLower().Contains(c.ToLower())){
                    _found= true;
                    break;
                }
            }

            return _found;
        }
    
        public static void PrintHeader(string[] h) {
            foreach (var col in h)
            {   
                ColorConsole.Write($"{col}\t", ConsoleColor.Cyan);
            } 
            ColorConsole.WriteLine("");
        }

        public static void PrintRow(string[] r) {
            foreach (var col in r)
            {   
                ColorConsole.Write($"{col}\t", ConsoleColor.Blue);
            } 
            ColorConsole.WriteLine("");
        }
    
    }

}
