using Audio;
using UnityEngine;


public class ItemActionDisconnectPowerV2 : ItemAction
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ItemActionDisconnectPowerV2>();

    public class MyInventoryData : ItemActionAttackData
    {
        public bool StartDisconnect;

        public MyInventoryData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction) { }
    }

    public ItemActionWiringData GetWiringData(ItemActionData _actionData) => _actionData.invData.holdingEntity.inventory.holdingItemData.actionData[0] as ItemActionWiringData;

    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new MyInventoryData(_invData, _indexInEntityOfAction);
    }

    public override void ReadFrom(DynamicProperties _props)
    {
        base.ReadFrom(_props);
    }

    public override void StopHolding(ItemActionData _data)
    {
        ((MyInventoryData)_data).StartDisconnect = false;
    }

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        if (_bReleased && !(Time.time - _actionData.lastUseTime < Delay))
        {
            _actionData.lastUseTime = Time.time;
            ((MyInventoryData)_actionData).StartDisconnect = true;

            GetWiringData(_actionData).RemoveLastPoint();
        }
    }

    public override bool IsActionRunning(ItemActionData _actionData)
    {
        MyInventoryData myInventoryData = (MyInventoryData)_actionData;
        if (myInventoryData.StartDisconnect && Time.time - myInventoryData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast)
        {
            return true;
        }
        return false;
    }

    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        MyInventoryData myInventoryData = (MyInventoryData)_actionData;
        if (!myInventoryData.StartDisconnect || Time.time - myInventoryData.lastUseTime < AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast)
        {
            return;
        }
        myInventoryData.StartDisconnect = false;
        _ = (MyInventoryData)_actionData;
        ItemInventoryData invData = _actionData.invData;
        _ = invData.hitInfo.lastBlockPos;
        Vector3i blockPos = _actionData.invData.hitInfo.hit.blockPos;
        if (!invData.hitInfo.bHitValid || invData.hitInfo.tag.StartsWith("E_") || ((ItemActionConnectPowerV2)_actionData.invData.holdingEntity.inventory.holdingItem.Actions[0]).DisconnectWire((ItemActionWiringData)_actionData.invData.holdingEntity.inventory.holdingItemData.actionData[0]) || !myInventoryData.invData.world.CanPlaceBlockAt(blockPos, myInventoryData.invData.world.gameManager.GetPersistentLocalPlayer()))
        {
            return;
        }
        IPowered poweredBlock = GetPoweredBlock(invData);
        if (poweredBlock != null)
        {
            if (myInventoryData.invData.itemValue.MaxUseTimes > 0 && myInventoryData.invData.itemValue.UseTimes >= (float)myInventoryData.invData.itemValue.MaxUseTimes)
            {
                EntityPlayerLocal player = _actionData.invData.holdingEntity as EntityPlayerLocal;
                if (item.Properties.Values.ContainsKey(ItemClass.PropSoundJammed))
                {
                    Manager.PlayInsidePlayerHead(item.Properties.Values[ItemClass.PropSoundJammed]);
                }
                GameManager.ShowTooltip(player, "ttItemNeedsRepair");
                return;
            }
            if (myInventoryData.invData.itemValue.MaxUseTimes > 0)
            {
                _actionData.invData.itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, _actionData.invData.itemValue, 1f, invData.holdingEntity, null, _actionData.invData.itemValue.ItemClass.ItemTags);
                HandleItemBreak(_actionData);
            }
            _actionData.invData.holdingEntity.RightArmAnimationAttack = true;
            poweredBlock.RemoveParentWithWiringTool(_actionData.invData.holdingEntity.entityId);
        }
        else
        {
            ((ItemActionConnectPowerV2)_actionData.invData.holdingEntity.inventory.holdingItem.Actions[0]).DisconnectWire((ItemActionWiringData)_actionData.invData.holdingEntity.inventory.holdingItemData.actionData[0]);
        }
    }

    public IPowered GetPoweredBlock(ItemInventoryData data)
    {
        Block block = data.world.GetBlock(data.hitInfo.hit.blockPos).Block;
        if (block is BlockPowered || block is BlockPowerSource)
        {
            Vector3i blockPos = data.hitInfo.hit.blockPos;
            ChunkCluster chunkCluster = data.world.ChunkClusters[data.hitInfo.hit.clrIdx];
            if (chunkCluster == null)
            {
                return null;
            }
            Chunk chunk = (Chunk)chunkCluster.GetChunkSync(World.toChunkXZ(blockPos.x), blockPos.y, World.toChunkXZ(blockPos.z));
            if (chunk == null)
            {
                return null;
            }
            TileEntity tileEntity = chunk.GetTileEntity(World.toBlock(blockPos));
            if (tileEntity == null)
            {
                if (block is BlockPowered)
                {
                    tileEntity = (block as BlockPowered).CreateTileEntity(chunk);
                }
                else if (block is BlockPowerSource)
                {
                    tileEntity = (block as BlockPowerSource).CreateTileEntity(chunk);
                }
                tileEntity.localChunkPos = World.toBlock(blockPos);
                BlockEntityData blockEntity = chunk.GetBlockEntity(blockPos);
                if (blockEntity != null)
                {
                    ((TileEntityPowered)tileEntity).BlockTransform = blockEntity.transform;
                }
                ((TileEntityPowered)tileEntity).InitializePowerData();
                chunk.AddTileEntity(tileEntity);
            }
            return tileEntity as IPowered;
        }
        return null;
    }
}
