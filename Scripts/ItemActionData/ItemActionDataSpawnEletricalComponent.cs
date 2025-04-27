public class ItemActionDataSpawnEletricalComponent : ItemActionAttackData
{
    public ElectricalComponentInstance component { get; set; }

    public float lastRotationTime;

    public BlockFace blockFace = BlockFace.None;

    public ItemActionDataSpawnEletricalComponent(ItemInventoryData _invData, int _indexInEntityOfAction)
        : base(_invData, _indexInEntityOfAction) { }
}