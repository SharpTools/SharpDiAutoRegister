namespace Microsoft.Extensions.DependencyInjection {
    public static class IServiceCollectionExtensions {

        public static IRegistrationOfAssemblies ForInterfacesMatching(this IServiceCollection serviceCollection,
                                                                      string regex) {
            var registration = new Registration(serviceCollection) {
                Regex = regex
            };
            return registration;
        }
    }
}
