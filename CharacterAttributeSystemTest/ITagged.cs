namespace CharacterAttributeSystemTest;

public interface ITagged
{
    IEnumerable<string> GetTags();
    bool HasTag(string tag);
    bool HasIntersectedTag(IEnumerable<string> tags);

}