using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricalComponentInstance : MonoBehaviour
{
    public class NodeInstance : MonoBehaviour
    {
        public ElectricalComponent.Node node;

        public ElectricalComponentInstance parent;

        public MeshRenderer meshRenderer;

        public Color Color
        {
            get => meshRenderer.material.color;
            set => meshRenderer.material.color = value;
        }

        public void Show(bool value)
        {
            meshRenderer.enabled = value;
        }
    }

    private static readonly Logging.Logger logger = Logging.CreateLogger<ElectricalComponentInstance>();

    private readonly List<NodeInstance> nodeInstances = new List<NodeInstance>();

    private ElectricalComponent Component { get; set; }

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
        componentInstance.Init(component);

        return componentInstance;
    }

    public ElectricalComponentInstance Clone()
    {
        var clone = Instantiate(this);

        clone.transform.position = this.transform.position;
        clone.transform.rotation = this.transform.rotation;
        clone.Init(this.Component);

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
            var nodeInstance = transform.gameObject.AddComponent<NodeInstance>();

            nodeInstance.meshRenderer = transform.GetComponent<MeshRenderer>();
            nodeInstance.meshRenderer.material = Config.MaterialSpritesDefault;
            nodeInstance.node = Component.nodes[transform.name];
            nodeInstance.parent = this;

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
            nodeInstance.Show(value);
        }
    }

    public void SetNodesColor(Color color)
    {
        foreach (var nodeInstance in nodeInstances)
        {
            nodeInstance.Color = color;
        }
    }

    public void Rotate(Vector3 axis, float angle) => transform.Rotate(axis, angle);

    public void Cleanup()
    {
        Object.Destroy(gameObject);
    }
}