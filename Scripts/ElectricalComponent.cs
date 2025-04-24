using UnityEngine;

public class ElectricalComponent
{
    private Transform transform;

    public ElectricalComponent(Transform transform)
    {
        this.transform = transform;
    }

    public void Cleanup()
    {
        if (transform != null && transform.gameObject != null)
        {
            Object.DestroyImmediate(transform.gameObject);
        }
    }
}