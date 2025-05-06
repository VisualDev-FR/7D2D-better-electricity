using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricalComponentInstance : MonoBehaviour
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ElectricalComponentInstance>();

    private readonly List<ElectricalNodeInstance> nodeInstances = new List<ElectricalNodeInstance>();

    private ElectricalComponent Component { get; set; }

    private ItemClass itemClass;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Quaternion Rotation
    {
        get => transform.rotation;
        set => transform.rotation = value;
    }

    public bool IsActive
    {
        get => transform.gameObject.activeSelf;
        set => transform.gameObject.SetActive(value);
    }

    public static ElectricalComponentInstance Create(ItemClass itemClass)
    {
        var prefab = DataLoader.LoadAsset<GameObject>(itemClass.MeshFile);
        var instance = Instantiate(prefab);

        var component = ElectricalComponentManager.Instance.GetComponent(itemClass.Name);
        var componentInstance = instance.AddComponent<ElectricalComponentInstance>();

        componentInstance.itemClass = itemClass;
        componentInstance.Init(component);

        return componentInstance;
    }

    public ElectricalComponentInstance Clone()
    {
        var clone = Create(itemClass);

        clone.Position = Position;
        clone.Rotation = Rotation;

        return clone;
    }

    private void Init(ElectricalComponent component)
    {
        Component = component;

        var nodeTransform = UnityUtils.FindByName(transform, "nodes");

        Assert.IsNotNull(Component);
        Assert.IsNotNull(nodeTransform);

        for (int i = 0; i < nodeTransform.childCount; i++)
        {
            var transform = nodeTransform.GetChild(i);
            var nodeInstance = transform.gameObject.AddComponent<ElectricalNodeInstance>();

            nodeInstance.meshRenderer = transform.GetComponent<MeshRenderer>();
            nodeInstance.meshRenderer.material = Config.MaterialSpritesDefault;
            nodeInstance.node = Component.nodes[transform.name];
            nodeInstance.parent = this;

            logger.Debug($"Created node instance: {nodeInstance.name}, with node: {nodeInstance.node}");


            Assert.IsNotNull(nodeInstance.node, $"name: {transform.name}"); // OK

            var nodeInstanceRetreived = transform.GetComponent<ElectricalComponentInstance>();

            Assert.IsNotNull(nodeInstance.node); // OK ou pas OK ?

            nodeInstances.Add(nodeInstance);
        }

        ShowNodes(false);
        SetNodesColor(Config.nodePreviewColor);
    }

    public virtual void OnSpawn()
    {
        Component.OnSpawn(this);
    }

    public void ShowNodes(bool value)
    {
        foreach (var nodeInstance in nodeInstances)
        {
            nodeInstance?.Show(value);
        }
    }

    public void SetNodesColor(Color color)
    {
        foreach (var nodeInstance in nodeInstances)
        {
            if (nodeInstance != null)
            {
                nodeInstance.Color = color;
            }
        }
    }

    public void Rotate(Vector3 axis, float angle) => transform.Rotate(axis, angle);

    public void Cleanup()
    {
        Object.Destroy(gameObject);
    }
}