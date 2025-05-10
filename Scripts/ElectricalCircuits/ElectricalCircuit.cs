using System.Collections.Generic;
using UnityEngine;

public class ElectricalCircuit : MonoBehaviour
{
    private readonly Dictionary<int, ElectricalNodeInstance> nodes = new Dictionary<int, ElectricalNodeInstance>();

    private readonly Dictionary<int, ElectricalWire> wires = new Dictionary<int, ElectricalWire>();

    public ElectricalCircuit Create()
    {
        var gameObject = new GameObject();
        var circuit = gameObject.AddComponent<ElectricalCircuit>();

        return circuit;
    }
}