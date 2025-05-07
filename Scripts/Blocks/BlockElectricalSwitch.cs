public class BlockElectricalSwitch : Block
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<BlockElectricalSwitch>();

    public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);

        var transform = _ebcd.transform;

        ElectricalComponentInstance.Create(this, transform);
    }
}