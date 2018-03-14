# SharpDiAutoRegister
Simple way to auto-register interfaces to their implementations with asp.net core default dependency injection.

## Nuget

`Install-Package SharpDiAutoRegister`

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

SharpDiAutoRegister **will not** override a pre-registered interface, so if you want a custom behavior for some of them, just register them **before** calling SharpDiAutoRegister methods.

Happy coding!
