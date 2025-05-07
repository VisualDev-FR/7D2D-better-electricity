using UnityEngine;
using ItemActionDataConnectPower = ItemActionConnectPowerV2.ActionData;


public class ItemActionDisconnectPowerV2 : ItemAction
{
    public class ItemActionDataDisconnectPower : ItemActionAttackData
    {
        public bool StartDisconnect;

        public ItemActionDataDisconnectPower(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction) { }
    }

    private static readonly Logging.Logger logger = Logging.CreateLogger<ItemActionDisconnectPowerV2>();

    public ItemActionDataConnectPower GetItemActionDataConnectPower(ItemActionData _actionData) => _actionData.invData.holdingEntity.inventory.holdingItemData.actionData[0] as ItemActionDataConnectPower;

    public ItemActionConnectPowerV2 GetItemActionConnectPowerV2(ItemActionData _actionData) => _actionData.invData.holdingEntity.inventory.holdingItem.Actions[0] as ItemActionConnectPowerV2;

    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ItemActionDataDisconnectPower(_invData, _indexInEntityOfAction);
    }

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        var actionData = _actionData as ItemActionDataDisconnectPower;

        if (!_bReleased || Time.time - _actionData.lastUseTime < Delay)
            return;

        if (DisconnectWireSection(actionData))
            return;

        if (DisconnectLookedAtWire(actionData))
            return;
    }

    private bool DisconnectLookedAtWire(ItemActionData actionData)
    {
        var targetWire = GetTargetWire(actionData);

        logger.Debug($"targetWire: {targetWire}");

        if (targetWire != null)
        {
            ElectricalWireManager.Instance.RemoveWire(targetWire);
            actionData.lastUseTime = Time.time;
            return true;
        }

        return false;
    }

    private bool DisconnectWireSection(ItemActionData actionData)
    {
        var itemActionConnect = GetItemActionConnectPowerV2(actionData);
        var itemActionDataConnect = GetItemActionDataConnectPower(actionData);

        if (!itemActionDataConnect.IsWiring)
            return false;

        if (InputUtils.ShiftKeyPressed)
        {
            itemActionConnect.StopWiring(itemActionDataConnect, false);
        }
        else
        {
            itemActionConnect.RemoveLastPoint(itemActionDataConnect);
        }

        actionData.lastUseTime = Time.time;

        return true;
    }

    public ElectricalWire GetTargetWire(ItemActionData actionData)
    {
        var player = actionData.invData.holdingEntity as EntityPlayer;
        var collider = RayCastUtils.GetLookedAtCollider(player);

        if (collider == null || collider.gameObject == null)
            return null;

        return collider.gameObject.GetComponent<ElectricalWireSection>()?.Parent;
    }

}
