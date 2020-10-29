using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using errchk.plugins;

namespace errchk.Helpers
{
    public class Plugins {
        public IEnumerable<Plugin> _Plugins { get;set; }

        public Plugins()
        {
            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<Plugin>()
                .Export<Plugin>()
                .Shared();

            var assemblies = new[] { typeof(Plugin).Assembly };
            var configuration = new ContainerConfiguration()
                .WithAssemblies(assemblies, conventions);

            using (var container= configuration.CreateContainer())
            {
                _Plugins = container.GetExports<Plugin>();
            }
        }
    }
}