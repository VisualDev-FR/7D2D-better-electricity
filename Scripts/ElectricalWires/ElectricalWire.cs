using System.Collections.Generic;
using UnityEngine;

public class ElectricalWire
{
    private readonly List<ElectricalWireSection> sections = new List<ElectricalWireSection>();

    public readonly Transform transform = new GameObject().transform;

    public Transform Parent
    {
        get => transform.parent;
        set => transform.parent = value;
    }

    public void AddSection(Vector3 start, Vector3 end)
    {
        sections.Add(new ElectricalWireSection(this, start, end));
    }

    public void RemoveLast()
    {
        if (sections.Count > 0)
        {
            var lastIndex = sections.Count - 1;
            var lastSection = sections[lastIndex];

            lastSection.Cleanup();
            sections.RemoveAt(lastIndex);
        }
    }

    public void Cleanup()
    {
        Object.DestroyImmediate(transform.gameObject);
    }
}