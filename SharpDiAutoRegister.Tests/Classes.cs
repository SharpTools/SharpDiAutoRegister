namespace SharpDiAutoRegister.Tests {
    public interface IFoo {}
    public class Foo : IFoo {}
    public interface IGeneric<T> {}
    public class Generic : IGeneric<Foo> { }
    public interface IMapper<TFrom, TTo> { }

    public class MapperIntString : IMapper<int, string> { }
    public class MapperIntDecimal : IMapper<int, decimal> { }
}
