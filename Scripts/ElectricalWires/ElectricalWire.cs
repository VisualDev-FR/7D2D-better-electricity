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

    public int SectionsCount => sections.Count;

    public Vector3 GetLastPoint()
    {
        if (sections.Count == 0)
            return Vector3.zero;

        return sections[sections.Count - 1].end;
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
        Object.Destroy(transform.gameObject);
    }
}