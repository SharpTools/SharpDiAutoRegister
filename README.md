# SharpDiAutoRegister
Simple way to auto-register interfaces to their implementations with asp.net core default dependency injection.

## How to use

In your `ConfigureServices` method in **Startup.cs**, just use some regex to match the interfaces you want to register and you are done. **SharpDiAutoRegister** will scan the Assemblies you provided and register the first class it finds that implements that interface.

Ex:
```cs

services.ForInterfacesMatching("^I[a-zA-z]+Repository$")
        .OfAssemblies(Assembly.GetExecutingAssembly())
        .AddSingletons();

services.ForInterfacesMatching("^IRepository")
        .OfAssemblies(Assembly.GetExecutingAssembly())
        .AddTransients();

//and so on...
```

Happy coding!
