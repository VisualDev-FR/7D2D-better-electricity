using System.Collections.Generic;
using UnityEngine;

public class ElectricalWireManager
{
    public static readonly ElectricalWireManager Instance = new ElectricalWireManager();

    public readonly HashSet<ElectricalWire> spawnedWires = new HashSet<ElectricalWire>();

    private GameObject gameObject = new GameObject();

    public void AddWire(ElectricalWire wire)
    {
        spawnedWires.Add(wire);
        wire.Parent = gameObject.transform;
    }

    public void RemoveWire(ElectricalWire wire)
    {
        spawnedWires.Remove(wire);
        wire.Cleanup();
    }

    public void Cleanup()
    {
        Object.Destroy(gameObject);
        spawnedWires.Clear();

        gameObject = new GameObject();
    }
}