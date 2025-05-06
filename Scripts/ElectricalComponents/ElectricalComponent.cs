using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Assertions;


public abstract class ElectricalComponent
{
    public string Name { get; set; }

    public int PowerLoss { get; set; }

    public readonly Dictionary<string, ElectricalNode> nodes = new Dictionary<string, ElectricalNode>();

    public virtual void Init(DynamicProperties properties)
    {
        PowerLoss = properties.GetInt("PowerLoss");
    }

    public virtual void CreateNode(XElement element)
    {
        var node = new ElectricalNode()
        {
            Name = element.Attribute("name").Value,
            nodeType = ModUtils.ParseEnum<ElectricalNode.NodeType>(element.Attribute("type").Value),
            Parent = this,
        };

        if (nodes.ContainsKey(node.Name))
        {
            throw new System.Exception($"Duplicate node name: '{node.Name}'");
        }

        Assert.IsNotNull(node);

        nodes[node.Name] = node;
    }

    public virtual void OnSpawn(ElectricalComponentInstance instance) { }

    public virtual void OnPickup(ElectricalComponentInstance instance) { }
}