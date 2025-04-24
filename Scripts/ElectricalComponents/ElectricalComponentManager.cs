using System.Collections.Generic;
using System.IO;


public class ElectricalComponentManager
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ElectricalComponentManager>();

    public static readonly ElectricalComponentManager Instance = new ElectricalComponentManager();

    public const string filename = "electricalcomponents.xml";

    private readonly Dictionary<string, ElectricalComponent> nameToComponent = new Dictionary<string, ElectricalComponent>();

    private readonly List<ElectricalComponentInstance> spawnedComponents = new List<ElectricalComponentInstance>();

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

    public void UpdateNodeVisibility(EntityPlayer player)
    {
        foreach (var component in spawnedComponents)
        {
            component.UpdateNodeVisibility(player);
        }
    }
}