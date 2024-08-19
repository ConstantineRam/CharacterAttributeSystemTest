namespace CharacterAttributeSystemTest;

[AttributeUsage(AttributeTargets.Class)]
public class DirectoryPathAttribute(string directoryPath) : Attribute
{
    public string DirectoryPath { get; } = directoryPath;
}