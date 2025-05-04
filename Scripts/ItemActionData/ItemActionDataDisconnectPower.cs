public class ItemActionDataDisconnectPower : ItemActionAttackData
{
    public bool StartDisconnect;

    public ItemActionDataDisconnectPower(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction) { }
}