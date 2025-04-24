using UnityEngine;

public class ElectricalComponentInstance
{
    public ElectricalComponent Component { get; private set; }

    public Transform Transform { get; set; }

    public ElectricalComponentInstance(string name, Transform transform)
    {
        this.Component = ElectricalComponentManager.Instance.GetComponent(name);
        this.Transform = transform;
    }

    public void OnSpawn() { }

    public void Cleanup()
    {
        if (Transform != null && Transform.gameObject != null)
        {
            Object.DestroyImmediate(Transform.gameObject);
        }
    }
}