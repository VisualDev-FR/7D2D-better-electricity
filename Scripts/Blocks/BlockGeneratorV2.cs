public class BlockGeneratorV2 : BlockGenerator
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<BlockGeneratorV2>();

    public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);

        ElectricalComponentManager.Instance.CreateComponent(this, _ebcd.transform, _blockPos);
    }

    public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);

        logger.Debug("block loaded");
    }

    public override void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);

        logger.Debug("block unloaded");
    }

    public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);

        ElectricalComponentManager.Instance.RemoveComponent(_blockPos);
    }
}