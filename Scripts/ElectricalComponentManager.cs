using System.Collections.Generic;
using UnityEngine;


public class ElectricalComponentManager
{
    public static readonly ElectricalComponentManager Instance = new ElectricalComponentManager();

    private readonly List<ElectricalComponent> components = new List<ElectricalComponent>();

    private World world => GameManager.Instance.World;

    public void AddComponentToWorld(ElectricalComponent component)
    {
        components.Add(component);
    }

    public void Cleanup()
    {
        foreach (var comp in components)
        {
            comp.Cleanup();
        }

        components.Clear();
    }
}