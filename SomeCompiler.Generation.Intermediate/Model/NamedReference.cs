using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model;

public class NamedReference : Reference
{
    public NamedReference(string value)
    {
        Value = value;
    }

    public string Value { get; }

    protected bool Equals(NamedReference other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((NamedReference) obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
}