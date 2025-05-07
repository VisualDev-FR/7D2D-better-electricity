using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class ElectricalComponentFromXml
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ElectricalComponentFromXml>();

    public static IEnumerable<ElectricalComponent> CreateComponents(XmlFile xmlFile)
    {
        XElement root = xmlFile.XmlDoc.Root;

        logger.Debug($"{root.Elements("component").Count()} components found");

        foreach (var element in root.Elements("component"))
        {
            yield return CreateComponent(element);
        }
    }

    private static ElectricalComponent CreateComponent(XElement node)
    {
        var name = node.Attribute("name").Value;
        var properties = ParseProperties(node);
        var type = Type.GetType(properties.Values["Class"]);
        var component = Activator.CreateInstance(type) as ElectricalComponent;

        component.Name = name;
        component.Init(properties);

        foreach (var element in node.Elements("node"))
        {
            component.CreateNode(element);
        }

        return component;
    }

    private static DynamicProperties ParseProperties(XElement node)
    {
        var properties = new DynamicProperties();

        foreach (var property in node.Elements("property"))
        {
            properties.Add(property);
        }

        return properties;
    }
}