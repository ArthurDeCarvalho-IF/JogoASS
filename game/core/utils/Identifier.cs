public sealed class Identifier
{
    public string Namespace { get; private set; }
    public string Path { get; private set; }

    public Identifier(string ns, string path)
    {
        Namespace = ns;
        Path = path;
    }
    public override string ToString()
    {
        return $"{Namespace}:{Path}";
    }

    public static Identifier FromString(string identifierString)
    {
        var parts = identifierString.Split(':');
        if (parts.Length != 2)
        {
            throw new System.FormatException("Invalid identifier format. Expected 'namespace:path'.");
        }
        return new Identifier(parts[0], parts[1]);
    }
}