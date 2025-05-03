namespace AIBridges.Attributes;

public class VersionAttribute : Attribute
{
    public string Version { get; }

    public VersionAttribute(string version)
    {
        Version = version;
    }
}