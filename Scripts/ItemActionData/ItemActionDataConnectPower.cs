public class ItemActionDataConnectPower : ItemActionAttackData
{
    public bool IsWiring { get; set; }

    public ElectricalWirePreview wirePreview;

    public ElectricalWire wire;

    public ItemActionDataConnectPower(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction) { }

}
