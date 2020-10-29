using System;
using System.IO;
using System.Reflection;

namespace errchk.plugins
{
    public abstract class Plugin : IPlugin
    {
        public abstract string Key { get; }

        public abstract string Name { get; }
        public abstract Type Type { get; }

        public string Version {
            get {
                Version version = Assembly.GetEntryAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        public abstract (int Errors,int Warnings) Run(string path, bool warnings, string customerId);
    }

}