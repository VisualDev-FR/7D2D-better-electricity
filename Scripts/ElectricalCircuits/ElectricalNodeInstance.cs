using UnityEngine;
using UnityEngine.Assertions;

public class ElectricalNodeInstance : MonoBehaviour
{
    public ElectricalNode node;

    public ElectricalComponentInstance parent;

    public MeshRenderer meshRenderer;

    public Vector3 Position => transform.position;

    public ElectricalNode.NodeType NodeType => node.nodeType;

    public Color Color
    {
        get => meshRenderer.material.color;
        set => meshRenderer.material.color = value;
    }

    public void Show(bool value)
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = value;
        }
    }

    public bool CanConnectWith(ElectricalNodeInstance other)
    {
        Assert.IsNotNull(node, "node is null");
        Assert.IsNotNull(other, "other is null");

        if (NodeType == ElectricalNode.NodeType.Dual)
            return true;

        if (NodeType != other.NodeType)
            return true;

        return false;
    }
}
