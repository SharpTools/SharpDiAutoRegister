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
            _serviceCollection.ForInterfacesMatching("^IFo")
                              .OfAssemblies(Assembly.GetExecutingAssembly())
                              .AddSingletons();
            var sp = _serviceCollection.BuildServiceProvider();
            var foo = sp.GetService<IFoo>();
            Assert.IsType<Foo>(foo);
        }

        [Fact]
        public void Should_reuse_provided_concrete_objects() {
            _serviceCollection.ForInterfacesMatching("^IFeature")
                              .OfAssemblies(Assembly.GetExecutingAssembly())
                              .UseWhenPossible(new Same())
                              .AddSingletons();
            var sp = _serviceCollection.BuildServiceProvider();

            var foo1 = sp.GetService<IFeature1>();
            var foo2 = sp.GetService<IFeature2>();
            Assert.IsType<Same>(foo1);
            Assert.Same(foo1, foo2);
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
