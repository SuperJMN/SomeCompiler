namespace SomeCompiler.Binding.Model;

public class BoundType
{
    public string Name { get; }

    private BoundType(string name)
    {
        Name = name;
    }

    public static BoundType Int => new("int");
    public static BoundType Char => new("char");

    public static BoundType Parse(string name)
    {
        return new[] { Int, Char }.First(x => x.Name == name);
    }
}