using System.Collections.Generic;

public class ElectricalWireManager
{
    public static readonly ElectricalWireManager Instance = new ElectricalWireManager();

    public readonly List<ElectricalWire> spawnedWires = new List<ElectricalWire>();

    public void RegisterWire(ElectricalWire wire)
    {
        spawnedWires.Add(wire);
    }

    public void Cleanup()
    {
        foreach (var wire in spawnedWires)
        {
            wire.Cleanup();
        }

        spawnedWires.Clear();
    }
}