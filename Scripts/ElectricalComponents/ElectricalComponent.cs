using System.Collections.Generic;
using System.Xml.Linq;


public abstract class ElectricalComponent
{
    public class Node
    {
        public ElectricalComponent Parent { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsInput => Type == "input" || Type == "dual";

        public bool IsOutput => Type == "output" || Type == "dual";
    }

    public string Name { get; set; }

    public int PowerLoss { get; set; }

    public readonly Dictionary<string, Node> nodes = new Dictionary<string, Node>();

    public virtual void Init(DynamicProperties properties)
    {
        PowerLoss = properties.GetInt("PowerLoss");
    }

    public virtual void CreateNode(XElement element)
    {
        var node = new Node()
        {
            Name = element.Attribute("name").Value,
            Type = element.Attribute("type").Value,
            Parent = this,
        };

        if (nodes.ContainsKey(node.Name))
        {
            throw new System.Exception($"Duplicate node name: '{node.Name}'");
        }

        nodes[node.Name] = node;
    }

    public virtual void OnSpawn(ElectricalComponentInstance instance) { }

    public virtual void OnPickup(ElectricalComponentInstance instance) { }
}