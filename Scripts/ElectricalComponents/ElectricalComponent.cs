using System.Collections.Generic;
using System.Xml.Linq;

public class ElectricalComponent
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

    public readonly List<Node> nodes = new List<Node>();

    public void Init(DynamicProperties properties)
    {
        PowerLoss = properties.GetInt("PowerLoss");
    }

    public void CreateNode(XElement element)
    {
        var connector = new Node()
        {
            Name = element.Attribute("name").Value,
            Type = element.Attribute("type").Value,
            Parent = this,
        };

        nodes.Add(connector);
    }
}