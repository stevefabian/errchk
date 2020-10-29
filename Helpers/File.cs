using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using errchk.Models;
using errchk.plugins;
using Gooddogs;
using Microsoft.VisualBasic.FileIO;

namespace errchk.Helpers {

    public static class File {

        public static List<Models.FileInfo> GetFileList(string path, string mask, string processdate) {
            var _files = new List<Models.FileInfo>();

            foreach (var item in Directory.GetFiles(path, $"*{mask}*"))
            {
                var fileparts = item.Split('_');

                var _type = fileparts[3];
                var _date = fileparts[4].Split('-')[0];

                if (_date == processdate) {
                    _files.Add(new Models.FileInfo() {
                        ErrorDate = _date,
                        Filename = item,
                        Type = _type,
                        FileType = _type
                    });
                }
            }


            return _files;
        }

        public static List<Models.FileInfo> GetFileList(string path, string mask, DateTime startDate, DateTime endDate) {
            var _files = new List<Models.FileInfo>();

            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles()
                .OrderBy(x => x.CreationTime);

            foreach (var item in files)
            {
                if (item.CreationTime >= startDate && item.CreationTime <= endDate.AddDays(1)) {
                    var fileparts = item.FullName.Split('_');

                    var _type = fileparts[4];
                    var _date = fileparts[5].Substring(0,8);

                    _files.Add(new Models.FileInfo() {
                        ErrorDate = _date,
                        Filename = item.FullName,
                        Type = _type,
                        FileType = _type
                    });
                }        
            }


            return _files;
        }
    }
}