using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;

namespace SharpDiAutoRegister.Tests {
    public class ServiceCollectionsExtensionsTests {
        private IServiceCollection _serviceCollection;

        public ServiceCollectionsExtensionsTests() {
            _serviceCollection = new ServiceCollection();
        }

        [Fact]
        public void Should_work_for_simple_interface() {
            _serviceCollection.ForInterfacesMatching("^IF")
                              .OfAssemblies(Assembly.GetExecutingAssembly())
                              .AddSingletons();
            var sp = _serviceCollection.BuildServiceProvider();
            var foo = sp.GetService<IFoo>();
            Assert.IsType<Foo>(foo);
        }

        [Fact]
        public void Should_work_with_generics() {
            _serviceCollection.ForInterfacesMatching("^IMapper")
                              .OfAssemblies(Assembly.GetExecutingAssembly())
                              .AddSingletons();

            var sp = _serviceCollection.BuildServiceProvider();
            var foo = sp.GetService<IMapper<int, string>>();
            Assert.IsType<MapperIntString>(foo);
        }
    }
}
