using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class ElectricalComponentManager
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ElectricalComponentManager>();

    public static readonly ElectricalComponentManager Instance = new ElectricalComponentManager();

    public const string filename = "electricalcomponents.xml";

    private readonly Dictionary<string, ElectricalComponent> nameToComponent = new Dictionary<string, ElectricalComponent>();

    private readonly List<ElectricalComponentInstance> spawnedComponents = new List<ElectricalComponentInstance>();

    private readonly Dictionary<int, ElectricalComponentInstance> blockToComponentInstance = new Dictionary<int, ElectricalComponentInstance>();

    public ElectricalComponent GetComponent(string name)
    {
        return nameToComponent[name];
    }

    public static void LoadComponents()
    {
        Instance.Cleanup();
        Instance.nameToComponent.Clear();

        var xmlPath = Path.GetFullPath(Config.ModPath + "/Config/electricalcomponents.xml");
        var createdComponents = 0;

        using (var reader = new StreamReader(xmlPath))
        {
            var xmlFile = new XmlFile(reader.BaseStream);

            foreach (var component in ElectricalComponentFromXml.CreateComponents(xmlFile))
            {
                Instance.nameToComponent[component.Name] = component;
                createdComponents++;
            }
        }

        logger.Info($"{createdComponents} electrical components created.");
    }

    public void SpawnComponent(ElectricalComponentInstance component)
    {
        spawnedComponents.Add(component);
        component.OnSpawn();
    }

    public void Cleanup()
    {
        foreach (var comp in spawnedComponents)
        {
            comp.Cleanup();
        }

        spawnedComponents.Clear();
    }

    public ElectricalComponentInstance CreateComponent(Block block, Transform transform, Vector3i blockPos)
    {
        var componentInstance = ElectricalComponentInstance.Create(block, transform);

        spawnedComponents.Add(componentInstance);
        blockToComponentInstance[blockPos.GetHashCode()] = componentInstance;

        return componentInstance;
    }

    public void RemoveComponent(Vector3i blockPos)
    {
        var hashcode = blockPos.GetHashCode();
        var componentInstance = blockToComponentInstance[hashcode];

        componentInstance.OnRemove();
        componentInstance.Cleanup();

        blockToComponentInstance.Remove(hashcode);
    }
}