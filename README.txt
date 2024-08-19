
Implementation;
    Solution content:
        CharacterAttributeSystemTest - attribute system project.
            When run executes "testIterations" amount of iterations of choosing random modifier and applying it.
            Results of the execution are printed into console.
        Test Suite - Unit test cover.
        
    Time system:
        Character Attribute System based on idea that game has time system. 
        For this test Mock Timing system with "ticks" was created. Ticks are a universal way to measure time in games and update objects based on priority.
        For example high prio items are updated every 100 ticks, low prio every 10000 ticks and so on.
        For the purpose of this work updates are run every tick.
        
    Adding New effects without modifying existing code: 
        Implemented via creating Types of Modifiers as children of serializable AttributeModifierParent class.
        Two types were implemented for the purpose of this work: ChangeValue (changes attribute value) and Purge (removes existing modifiers).
        Modifiers are loaded from json files in respective folders inside solution (\bin\Debug\net8.0\). 
        To add new modifier create a new json file in the folder.
        
        We can go further and make modifier Types also serializable and injected via .dll, so we can keep this part of codebase completely immutable,
        but it's overengineering for the purpose of this work.
 
    Application Of Change Value modifiers asynchronously:
        Modifiers don't apply / remove themselves immediately upon visiting Character attribute, but put self into queue to be dispatched upon update.
        It may be irrelevant for a isolated system as we have here, however in a real project it allows us to uncouple calculations from 
        moment of application to a moments when we are comfortable with our resources.  
         
        We are not limited to AttributeModifierParent as foundation for our modifiers and can work from IModifier if we would need something unique.
        
    Console Verbose:
        There are few Console.Writeln() added into different places of an app for the purpose of adding extra verbosity during execution or 
        to emulate error handling. 
             
What is omitted as unrelated to test task:
    Graphics, attributes serialization, error handling and logging replaced with Console.Writeln

Potential to expand:
    Powerful Glamour is applied to Corruption too. However, corruption is considered Negative attribute. It's a hint to implement
        negative tags. For example "!Eldritch", so specific modifier would not be applied to attribute with this tag.
    Calculation we performed during modifier visit can be detached and delayed even more.