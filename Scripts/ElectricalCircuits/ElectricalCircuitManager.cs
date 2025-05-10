using System.Collections.Generic;

public class ElectricalCircuitManager
{
    public ElectricalCircuitManager Instance = new ElectricalCircuitManager();

    public List<ElectricalCircuit> circuits = new List<ElectricalCircuit>();

    public Dictionary<int, ElectricalCircuit> nodeToCircuit = new Dictionary<int, ElectricalCircuit>();

    public Dictionary<int, ElectricalCircuit> wireToCircuit = new Dictionary<int, ElectricalCircuit>();
}