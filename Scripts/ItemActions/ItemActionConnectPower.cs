using UnityEngine;
using UnityEngine.Assertions;


public class ItemActionConnectPowerV2 : ItemAction
{
    public class ActionData : ItemActionAttackData
    {
        public bool IsWiring { get; set; }

        public ElectricalWirePreview wirePreview;

        public ElectricalWire wire;

        public ElectricalNodeInstance targetNode;

        public ElectricalComponentInstance targetComponent;

        public ActionData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction) { }

    }

    private static readonly Logging.Logger logger = Logging.CreateLogger<ItemActionConnectPowerV2>();

    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ActionData(_invData, _indexInEntityOfAction);
    }

    public override void StartHolding(ItemActionData _data)
    {
        base.StartHolding(_data);

        var actionData = _data as ActionData;
        var player = actionData.invData.holdingEntity as EntityPlayer;

        actionData.wire = new ElectricalWire();
        actionData.wirePreview = new ElectricalWirePreview(player);
    }

    public override void StopHolding(ItemActionData _data)
    {
        var actionData = _data as ActionData;

        StopWiring(actionData, true);
    }

    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        if (!(_actionData is ActionData actionData))
            return;

        UpdateTarget(actionData);

        if (!actionData.IsWiring)
            return;

        actionData.wirePreview.Update(actionData.invData.hitInfo);
    }

    private void UpdateTarget(ActionData actionData)
    {
        var player = actionData.invData.holdingEntity as EntityPlayer;

        ElectricalNodeInstance targetNode = null;
        ElectricalComponentInstance targetComponent = null;

        foreach (var hit in Physics.RaycastAll(player.GetLookRay(), 4f))
        {
            var node = hit.transform.GetComponent<ElectricalNodeInstance>();
            if (node != null)
            {
                targetNode = node;
                break;
            }

            var component = hit.transform.GetComponent<ElectricalComponentInstance>();
            if (component != null)
            {
                targetComponent = component;
                break;
            }
        }

        actionData.targetComponent?.ShowNodes(false);
        actionData.targetComponent?.SetNodesColor(Config.nodePreviewColor);
        actionData.targetComponent = null;
        actionData.targetNode = null;

        if (targetNode != null)
        {
            bool canConnect = actionData.wire.StartNode == null || targetNode.CanConnectWith(actionData.wire.StartNode);

            actionData.targetComponent = targetNode.parent;
            actionData.targetNode = targetNode;
            actionData.targetComponent.ShowNodes(true);
            actionData.targetNode.Color = canConnect ? new Color(0, 255, 0, 0.05f) : new Color(255, 0, 0, 0.05f);
        }
        else if (targetComponent != null)
        {
            actionData.targetComponent = targetComponent;
            actionData.targetComponent.ShowNodes(true);
        }
    }

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        if (!_bReleased || Time.time - _actionData.lastUseTime < Delay)
            return;

        var actionData = _actionData as ActionData;

        if (!actionData.IsWiring && actionData.targetNode != null)
        {
            StartWiring(actionData);
            return;
        }
        else if (actionData.IsWiring && actionData.targetNode != null)
        {
            ValidateWire(actionData);
            return;
        }
        else if (actionData.IsWiring && actionData.invData.hitInfo.bHitValid)
        {
            AddWiringNode(actionData);
            return;
        }

        ModUtils.UIDenied("ttRequiresCompatibleNode");
    }

    public override bool IsActionRunning(ItemActionData _actionData)
    {
        var actionData = _actionData as ActionData;

        if (actionData.IsWiring && Time.time - actionData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[actionData.invData.item.HoldType.Value].RayCast)
        {
            return true;
        }

        return false;
    }

    public bool DisconnectWire(ActionData _actionData)
    {
        return false;
    }

    private void StartWiring(ActionData actionData)
    {
        actionData.IsWiring = true;
        actionData.wire.StartNode = actionData.targetNode;
        actionData.wirePreview.Start = actionData.targetNode.Position;
        actionData.wirePreview.Update(actionData.invData.hitInfo);
        actionData.wirePreview.SetActive(true);
    }

    public void StopWiring(ActionData actionData, bool cleanupPreview)
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

    private void AddWiringNode(ActionData actionData)
    {
        var hitPos = RayCastUtils.CalcHitPos(actionData.invData.hitInfo, Config.wireOffset);

        actionData.wire.AddSection(actionData.wirePreview.Start, hitPos);
        actionData.wirePreview.Start = hitPos;
        actionData.IsWiring = true;
    }

    public void ValidateWire(ActionData actionData)
    {
        Assert.IsNotNull(actionData.wire);
        Assert.IsNotNull(actionData.wire.StartNode);
        Assert.IsNotNull(actionData.wire.StartNode.node);

        Assert.IsNotNull(actionData.targetNode);
        Assert.IsNotNull(actionData.targetNode.node);

        if (!actionData.wire.StartNode.CanConnectWith(actionData.targetNode))
        {
            ModUtils.UIDenied("ttRequiresCompatibleNode");
            return;
        }

        actionData.wire.EndNode = actionData.targetNode;
        actionData.wire.AddSection(actionData.wirePreview.Start, actionData.targetNode.Position);

        ElectricalWireManager.Instance.AddWire(actionData.wire);

        actionData.IsWiring = false;
        actionData.wire = new ElectricalWire();
        actionData.wirePreview.SetActive(false);
    }

    public void RemoveLastPoint(ActionData actionData)
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
