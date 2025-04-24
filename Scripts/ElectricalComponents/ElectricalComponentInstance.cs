using UnityEngine;

public class ElectricalComponentInstance
{
    private ElectricalComponent Component { get; set; }

    private Transform Transform { get; set; }

    private Transform NodeTransform { get; set; }

    public Quaternion Rotation
    {
        get => Transform.rotation;
        set => Transform.rotation = value;
    }

    public Vector3 Position
    {
        get => Transform.position;
        set => Transform.position = value;
    }

    public ElectricalComponentInstance(string name, Transform transform)
    {
        Component = ElectricalComponentManager.Instance.GetComponent(name);
        NodeTransform = UnityUtils.FindByName(transform, "nodes");
        Transform = transform;
    }

    public ElectricalComponentInstance Clone()
    {
        var clonedTransform = Object.Instantiate(Transform.gameObject).transform;

        return new ElectricalComponentInstance(Component.Name, clonedTransform)
        {
            Position = this.Position,
            Rotation = this.Rotation,
        };
    }

    public void OnSpawn()
    {
        Component.OnSpawn(this);
    }

    public void OnPickup()
    {
        Component.OnPickup(this);
    }

    public bool HasNullTransform() => Transform == null || Transform.gameObject == null;

    public void Rotate(Vector3 axis, float angle) => Transform.Rotate(axis, angle);

    public void ShowNodes(bool value) => NodeTransform.gameObject.SetActive(value);

    public void SetActive(bool value) => Transform.gameObject.SetActive(value);

    public bool IsActive() => Transform.gameObject.activeSelf;

    public void Cleanup()
    {
        if (Transform != null && Transform.gameObject != null)
        {
            Object.DestroyImmediate(Transform.gameObject);
        }

        Transform = null;
        NodeTransform = null;
    }
}