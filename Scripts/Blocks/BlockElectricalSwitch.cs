public class BlockElectricalSwitch : Block
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<BlockElectricalSwitch>();

    public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);

        var transform = _ebcd.transform;

        ElectricalComponentInstance.Create(this, transform);
    }

    public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
    }
}