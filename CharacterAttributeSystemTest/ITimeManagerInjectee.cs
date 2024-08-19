namespace CharacterAttributeSystemTest;

public interface ITimeManagerInjectee
{
    void InjectTimeManager(in ITimeManager timeManager);
}