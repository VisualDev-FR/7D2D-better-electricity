using UnityEngine;

public class ItemActionDataSpawnEletricalComponent : ItemActionAttackData
{
    public ElectricalComponentInstance component { get; set; }

    public Renderer[] PreviewRenderers;

    public bool ValidPosition;

    public Vector3 Position;

    public bool Placing;

    public float lastRotation;

    public BlockFace blockFace = BlockFace.None;

    public ItemActionDataSpawnEletricalComponent(ItemInventoryData _invData, int _indexInEntityOfAction)
        : base(_invData, _indexInEntityOfAction) { }
}