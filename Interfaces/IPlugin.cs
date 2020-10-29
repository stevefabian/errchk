using System;

namespace errchk.plugins
{
    public interface IPlugin {

        string Key { get; }

        string Name { get; }

        Type Type { get; }
        
        string Version { get; }

        (int Errors, int Warnings) Run(string path, bool warnings, string customerId);

    }
}