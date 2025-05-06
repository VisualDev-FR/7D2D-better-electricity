public class ElectricalNode
{
    public enum NodeType
    {
        Input = 0,
        Output = 0,
        Dual = 0,
    }

    public ElectricalComponent Parent { get; set; }

    public string Name { get; set; }

    public NodeType nodeType { get; set; }

    public bool IsInput => nodeType == NodeType.Input || nodeType == NodeType.Dual;

    public bool IsOutput => nodeType == NodeType.Output || nodeType == NodeType.Dual;
}