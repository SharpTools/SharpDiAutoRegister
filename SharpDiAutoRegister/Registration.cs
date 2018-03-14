using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.DependencyInjection {
    public class Registration : IRegistrationOfAssemblies, IRegistrationServiceLifetime {
        private IServiceCollection _serviceCollection;
        public string Regex { get; set; }
        public List<Assembly> Assemblies { get; set; }

        public ServiceLifetime ServiceLifetime = ServiceLifetime.Singleton;

        public Registration(IServiceCollection serviceCollection) {
            _serviceCollection = serviceCollection;
        }

        public IRegistrationServiceLifetime OfAssemblies(IEnumerable<Assembly> assemblies) {
            Assemblies = assemblies.ToList();
            return this;
        }

        public IRegistrationServiceLifetime OfAssemblies(Assembly assembly) {
            return OfAssemblies(new List<Assembly> { assembly });
        }

        public void AddSingletons() {
            ServiceLifetime = ServiceLifetime.Singleton;
            Register();
        }

        public void AddTransients() {
            ServiceLifetime = ServiceLifetime.Transient;
            Register();
        }

        public void AddScoped() {
            ServiceLifetime = ServiceLifetime.Scoped;
            Register();
        }

        private void Register() {
            var regex = new Regex(Regex);
            var interfaces = Assemblies.SelectMany(p => p.GetTypes())
                                       .SelectMany(p => p.GetInterfaces())
                                       .Where(p => regex.IsMatch(p.Name));
            foreach (var @interface in interfaces) {
                if (_serviceCollection.Any(s => s.ServiceType == @interface)) {
                    continue;
                }
                var impl = Assemblies.SelectMany(a => a.GetTypes())
                                     .FirstOrDefault(t => @interface.IsAssignableFrom(t) && !t.IsInterface);
                if (impl != null) {
                    switch(ServiceLifetime) {
                        case ServiceLifetime.Singleton:
                            _serviceCollection.AddSingleton(@interface, impl);
                            break;
                        case ServiceLifetime.Transient:
                            _serviceCollection.AddTransient(@interface, impl);
                            break;
                        case ServiceLifetime.Scoped:
                            _serviceCollection.AddScoped(@interface, impl);
                            break;
                    }
                }
            }
        }

    }

    public interface IRegistrationOfAssemblies {
        IRegistrationServiceLifetime OfAssemblies(IEnumerable<Assembly> assemblies);
        IRegistrationServiceLifetime OfAssemblies(Assembly assembly);
    }

    public interface IRegistrationServiceLifetime {
        void AddSingletons();
        void AddTransients();
        void AddScoped();
    }
}
