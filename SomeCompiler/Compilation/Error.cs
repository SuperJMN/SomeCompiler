namespace SomeCompiler.Compilation;

public class Error
{
    public Error(ErrorKind kind, string message)
    {
        Kind = kind;
        Message = message;
    }

    public ErrorKind Kind { get; }
    public string Message { get; }

    public override string ToString() => $"{nameof(Kind)}: {Kind}, {nameof(Message)}: {Message}";
}