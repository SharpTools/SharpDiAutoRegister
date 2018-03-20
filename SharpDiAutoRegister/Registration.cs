using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.DependencyInjection {
    public class Registration : IRegistrationOfAssemblies, 
                                IRegistrationReuseImplmentations, 
                                IRegistrationServiceLifetime {

        private IServiceCollection _serviceCollection;
        public string Regex { get; set; }
        public List<Assembly> Assemblies { get; set; }
        public Dictionary<Type, object> ReusableImplementations { get; set; } = new Dictionary<Type, object>();
        public ServiceLifetime ServiceLifetime = ServiceLifetime.Singleton;

        public Registration(IServiceCollection serviceCollection) {
            _serviceCollection = serviceCollection;
        }

        public IRegistrationReuseImplmentations OfAssemblies(IEnumerable<Assembly> assemblies) {
            Assemblies = assemblies.ToList();
            return this;
        }

        public IRegistrationReuseImplmentations OfAssemblies(Assembly assembly) {
            return OfAssemblies(new List<Assembly> { assembly });
        }

        public IRegistrationServiceLifetime UseWhenPossible(object obj) {
            ReusableImplementations.Add(obj.GetType(), obj);
            return this;
        }

        public IRegistrationServiceLifetime UseWhenPossible(List<object> objects) {
            objects.ForEach(obj => ReusableImplementations.Add(obj.GetType(), obj));
            return this;
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
            var implentations = new HashSet<Type>();
            var regex = new Regex(Regex);
            var interfaces = Assemblies.SelectMany(p => p.GetTypes())
                                       .SelectMany(p => p.GetInterfaces())
                                       .Where(p => regex.IsMatch(p.Name));
            foreach (var @interface in interfaces) {
                if (_serviceCollection.Any(s => s.ServiceType == @interface)) {
                    continue;
                }
                var impl = Assemblies.SelectMany(p => p.GetTypes())
                                     .FirstOrDefault(t => @interface.IsAssignableFrom(t)
                                                          && !t.IsInterface 
                                                          && !t.IsAbstract);
                if(impl == null) {
                    continue;
                }
                if(ReusableImplementations.ContainsKey(impl)) {
                    _serviceCollection.Add(new ServiceDescriptor(@interface, ReusableImplementations[impl]));
                }
                else {
                    _serviceCollection.Add(new ServiceDescriptor(@interface, impl, ServiceLifetime));
                }
            }
        }

        private bool HasParameterlessConstructor(Type type) {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }

    public interface IRegistrationOfAssemblies {
        IRegistrationReuseImplmentations OfAssemblies(IEnumerable<Assembly> assemblies);
        IRegistrationReuseImplmentations OfAssemblies(Assembly assembly);
    }

    public interface IRegistrationReuseImplmentations {
        void AddSingletons();
        void AddTransients();
        void AddScoped();
        IRegistrationServiceLifetime UseWhenPossible(object obj);
        IRegistrationServiceLifetime UseWhenPossible(List<object> objects);
    }

    public interface IRegistrationServiceLifetime {
        void AddSingletons();
        void AddTransients();
        void AddScoped();
    }
}
