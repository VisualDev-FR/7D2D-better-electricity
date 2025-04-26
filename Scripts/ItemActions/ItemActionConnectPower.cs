using UnityEngine;


public class ItemActionConnectPowerV2 : ItemAction
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ItemActionConnectPowerV2>();

    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ItemActionWiringData(_invData, _indexInEntityOfAction);
    }

    public override void ReadFrom(DynamicProperties _props)
    {
        base.ReadFrom(_props);
    }

    public override void StopHolding(ItemActionData _data)
    {
        logger.Debug("StopHolding");
        (_data as ItemActionWiringData).ClearWires();
    }

    public override void StartHolding(ItemActionData _data)
    {
        logger.Debug("StartHolding");

        base.StartHolding(_data);
        if (_data.invData.holdingEntity is EntityPlayerLocal)
        {
            ((ItemActionWiringData)_data).PlayerUI = LocalPlayerUI.GetUIForPlayer(_data.invData.holdingEntity as EntityPlayerLocal);
            WireManager.Instance.ToggleAllWirePulse(isPulseOn: true);
        }
    }

    public float GetDistance(Vector3 v1, Vector3 v2)
    {
        return (v2 - v1).magnitude;
    }

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        if (!_bReleased || Time.time - _actionData.lastUseTime < Delay)
            return;

        var connectPowerData = _actionData as ItemActionWiringData;

        if (!connectPowerData.IsWiring && connectPowerData.TryAddPoint())
        {
            connectPowerData.StartWiring();
        }
        else if (connectPowerData.IsWiring)
        {
            connectPowerData.TryAddPoint();
        }
    }

    public override bool IsActionRunning(ItemActionData _actionData)
    {
        ItemActionWiringData connectPowerData = (ItemActionWiringData)_actionData;
        if (connectPowerData.IsWiring && Time.time - connectPowerData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[connectPowerData.invData.item.HoldType.Value].RayCast)
        {
            return true;
        }
        return false;
    }

    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        if (!(_actionData is ItemActionWiringData actionData))
            return;

        if (actionData.invData.holdingEntity is EntityPlayer player)
            ElectricalComponentManager.Instance.UpdateNodeVisibility(player);

        if (!actionData.IsWiring)
            return;

        actionData.UpdateHitPos();

    }

    public Transform GetHandTransform(EntityAlive holdingEntity)
    {
        Transform transform = holdingEntity.RootTransform.Find("Graphics").FindInChilds(holdingEntity.GetRightHandTransformName(), onlyActive: true);

        if (transform != null && transform.childCount > 0)
        {
            return transform;
        }
        Transform transform3 = holdingEntity.RootTransform.Find("Camera").FindInChilds(holdingEntity.GetRightHandTransformName(), onlyActive: true);
        if (transform3 != null && transform3.childCount > 0)
        {
            return transform3;
        }
        return holdingEntity.emodel.GetRightHandTransform();
    }

    public void CheckForWireRemoveNeeded(EntityAlive _player, Vector3i _blockPos)
    {
        ItemActionWiringData connectPowerData = (ItemActionWiringData)_player.inventory.holdingItemData.actionData[1];
        if (connectPowerData.HasStartPoint && connectPowerData.StartPoint == _blockPos)
        {
            DisconnectWire(connectPowerData);
        }
    }

    public override ItemClass.EnumCrosshairType GetCrosshairType(ItemActionData _actionData)
    {
        if (_actionData.invData.hitInfo.bHitValid && (_actionData as ItemActionWiringData).isFriendly)
        {
            int num = (int)(Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance);
            if (_actionData.invData.hitInfo.hit.distanceSq <= (float)num)
            {
                Vector3i blockPos = _actionData.invData.hitInfo.hit.blockPos;
                Block block = _actionData.invData.world.GetBlock(blockPos).Block;
                if (block is BlockPowered)
                {
                    return ItemClass.EnumCrosshairType.PowerItem;
                }
                if (block is BlockPowerSource)
                {
                    return ItemClass.EnumCrosshairType.PowerSource;
                }
            }
        }
        return ItemClass.EnumCrosshairType.Plus;
    }

    public TileEntityPowered GetPoweredBlock(ItemInventoryData data)
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
            return tileEntity as TileEntityPowered;
        }
        return null;
    }

    public TileEntityPowered GetPoweredBlock(Vector3i tileEntityPos)
    {
        World world = GameManager.Instance.World;
        Block block = world.GetBlock(tileEntityPos).Block;
        if (block is BlockPowered || block is BlockPowerSource)
        {
            if (!(world.GetChunkFromWorldPos(tileEntityPos.x, tileEntityPos.y, tileEntityPos.z) is Chunk chunk))
            {
                return null;
            }
            TileEntity tileEntity = chunk.GetTileEntity(World.toBlock(tileEntityPos));
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
                tileEntity.localChunkPos = World.toBlock(tileEntityPos);
                BlockEntityData blockEntity = chunk.GetBlockEntity(tileEntityPos);
                if (blockEntity != null)
                {
                    ((TileEntityPowered)tileEntity).BlockTransform = blockEntity.transform;
                }
                ((TileEntityPowered)tileEntity).InitializePowerData();
                chunk.AddTileEntity(tileEntity);
            }
            return tileEntity as TileEntityPowered;
        }
        return null;
    }

    public bool DisconnectWire(ItemActionWiringData _actionData)
    {
        return false;
    }

    public void DecreaseDurability(ItemActionWiringData _actionData)
    {
        if (_actionData.invData.itemValue.MaxUseTimes > 0)
        {
            if (_actionData.invData.itemValue.UseTimes + 1f < (float)_actionData.invData.itemValue.MaxUseTimes)
            {
                _actionData.invData.itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, _actionData.invData.itemValue, 1f, _actionData.invData.holdingEntity, null, _actionData.invData.itemValue.ItemClass.ItemTags);
                HandleItemBreak(_actionData);
            }
            else
            {
                _actionData.invData.holdingEntity.inventory.DecHoldingItem(1);
            }
        }
    }
}
