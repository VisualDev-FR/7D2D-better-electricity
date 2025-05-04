using UnityEngine;


public class ItemActionConnectPowerV2 : ItemAction
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ItemActionConnectPowerV2>();

    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ItemActionDataConnectPower(_invData, _indexInEntityOfAction);
    }

    public override void StartHolding(ItemActionData _data)
    {
        base.StartHolding(_data);

        var actionData = _data as ItemActionDataConnectPower;
        var player = actionData.invData.holdingEntity as EntityPlayer;

        actionData.wire = new ElectricalWire();
        actionData.wirePreview = new ElectricalWirePreview(player);
    }

    public override void StopHolding(ItemActionData _data)
    {
        var actionData = _data as ItemActionDataConnectPower;

        StopWiring(actionData, true);
    }

    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        if (!(_actionData is ItemActionDataConnectPower actionData))
            return;

        var player = actionData.invData.holdingEntity as EntityPlayer;

        if (player == null)
            ElectricalComponentManager.Instance.UpdateNodeVisibility(player);

        if (!actionData.IsWiring)
            return;

        actionData.wirePreview.Update(actionData.invData.hitInfo);
    }

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        if (!_bReleased || Time.time - _actionData.lastUseTime < Delay)
            return;

        var actionData = _actionData as ItemActionDataConnectPower;
        var hitInfoValid = actionData.invData.hitInfo.bHitValid;

        if (hitInfoValid && !actionData.IsWiring)
        {
            StartWiring(actionData);
        }
        else if (hitInfoValid)
        {
            AddWiringNode(actionData);
        }
    }

    public override bool IsActionRunning(ItemActionData _actionData)
    {
        var actionData = _actionData as ItemActionDataConnectPower;

        if (actionData.IsWiring && Time.time - actionData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[actionData.invData.item.HoldType.Value].RayCast)
        {
            return true;
        }

        return false;
    }

    public bool DisconnectWire(ItemActionDataConnectPower _actionData)
    {
        return false;
    }

    private void StartWiring(ItemActionDataConnectPower actionData)
    {
        actionData.IsWiring = true;
        actionData.wirePreview.Start = RayCastUtils.CalcHitPos(actionData.invData.hitInfo, Config.wireOffset);
        actionData.wirePreview.Update(actionData.invData.hitInfo);
        actionData.wirePreview.SetActive(true);
    }

    public void StopWiring(ItemActionDataConnectPower actionData, bool cleanupPreview)
    {
        actionData.IsWiring = false;
        actionData.wire.RemoveAll();

        if (cleanupPreview)
        {
            actionData.wirePreview.Cleanup();
        }
        else
        {
            actionData.wirePreview.SetActive(false);
        }
    }

    private void AddWiringNode(ItemActionDataConnectPower actionData)
    {
        var hitPos = RayCastUtils.CalcHitPos(actionData.invData.hitInfo, Config.wireOffset);

        actionData.wire.AddSection(actionData.wirePreview.Start, hitPos);
        actionData.wirePreview.Start = hitPos;
        actionData.IsWiring = true;

        if (InputUtils.ShiftKeyPressed)
        {
            ValidateWire(actionData);
            return;
        }
    }

    public void ValidateWire(ItemActionDataConnectPower actionData)
    {
        ElectricalWireManager.Instance.AddWire(actionData.wire);

        actionData.IsWiring = false;
        actionData.wire = new ElectricalWire();
        actionData.wirePreview.SetActive(false);
    }

    public void RemoveLastPoint(ItemActionDataConnectPower actionData)
    {
        if (actionData.wire.SectionsCount > 0)
        {
            actionData.wirePreview.Start = actionData.wire.GetLastSection().StartPos;
            actionData.wire.RemoveLast();
        }
        else
        {
            actionData.IsWiring = false;
            actionData.wirePreview.SetActive(false);
        }
    }
}
