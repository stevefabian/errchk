using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using errchk.plugins;
using Gooddogs;
using ManyConsole;

namespace errchk
{
    class Program
    {
        [ImportMany]
        public static IEnumerable<Plugin> _plugins { get; set;}

        static int Main(string[] args)
        {
            ColorConsole.WriteLine("C&I PORTAL - PRODUCTION ERROR PARSER", ConsoleColor.Green);

            var commands = GetCommands();
            var result = ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);

            return result;
        }

        private static IEnumerable<ConsoleCommand> GetCommands() {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }

        private static void LoadPlugins() {
            var path = @"C:\repos\errchk.plugins\library\";
            ColorConsole.WriteInfo($"Scanning plugin folder : {path}");

            var conventions = new ConventionBuilder();
            conventions
                .ForTypesDerivedFrom<Plugin>()
                .Export<Plugin>()
                .Shared();

            var assemblies = Directory.GetFiles(path, "*.dll")
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);

            using (var container = configuration.CreateContainer())
            {
                try
                {
                    var plugins = container.GetExport<Plugin>();            
                }
                catch (System.Exception ex)
                {
                    ColorConsole.WriteError(ex.Message);
                }
            }
        }
    }
}