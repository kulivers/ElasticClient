namespace ElasticClient;

public enum ProcessingAttributeBehaviourType
{
    Processor,
    Input,
    Output,
    Factory
}

public class ProcessElementAttribute : Attribute
{
    private string Name { get; }
    public ProcessingAttributeBehaviourType Type { get; }

    public ProcessElementAttribute(string name, ProcessingAttributeBehaviourType type)
    {
        Name = name;
        Type = type;
    }
}