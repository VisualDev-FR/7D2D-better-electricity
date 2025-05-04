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

    public ElectricalWireSection GetLastSection()
    {
        if (sections.Count == 0)
            return null;

        return sections[sections.Count - 1];
    }

    public void AddSection(Vector3 start, Vector3 end)
    {
        sections.Add(ElectricalWireSection.Create(this, start, end));
    }

    public bool RemoveLast()
    {
        if (sections.Count > 0)
        {
            var lastIndex = sections.Count - 1;
            var lastSection = sections[lastIndex];

            Object.Destroy(lastSection.gameObject);
            sections.RemoveAt(lastIndex);

            return true;
        }

        return false;
    }

    public void RemoveAll()
    {
        foreach (var section in sections)
        {
            Object.Destroy(section.gameObject);
        }

        sections.Clear();
    }

    public void Cleanup()
    {
        Object.Destroy(transform.gameObject);
    }
}