# pragma warning disable IDE0019, IDE0031

using System.Collections.Generic;
using UnityEngine;


public class ItemActionSpawnElectricalComponent : ItemAction
{
    public class ItemActionDataSpawnEletricalComponent : ItemActionAttackData
    {
        public ElectricalComponentInstance componentInstance { get; set; }

        public float lastRotationTime;

        public BlockFace blockFace = BlockFace.None;

        public ItemActionDataSpawnEletricalComponent(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction) { }
    }

    private static readonly Logging.Logger logger = Logging.CreateLogger<ItemActionSpawnElectricalComponent>();

    public const int cColliderMask = 28901376;

    public const float rotationDelay = 0.2f;

    public const string propConnectors = "Connectors";

    public string entityToSpawn;

    public int entityClassId = -1;

    public Vector3 turretSize = new Vector3(0.5f, 0.5f, 0.5f);

    public Vector3 previewSize = new Vector3(1f, 1f, 1f);

    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ItemActionDataSpawnEletricalComponent(_invData, _indexInEntityOfAction);
    }

    public override bool AllowConcurrentActions() => false;

    public override void ReadFrom(DynamicProperties _props)
    {
        base.ReadFrom(_props);

        if (_props.Values.ContainsKey("Turret"))
            entityToSpawn = _props.Values["Turret"];

        if (_props.Values.ContainsKey("Scale"))
            turretSize = StringParsers.ParseVector3(_props.Values["Scale"]);

        if (_props.Values.ContainsKey("PreviewSize"))
            previewSize = StringParsers.ParseVector3(_props.Values["PreviewSize"]);

        foreach (KeyValuePair<int, EntityClass> item in EntityClass.list.Dict)
        {
            if (item.Value.entityClassName == entityToSpawn)
            {
                entityClassId = item.Key;
                break;
            }
        }
    }

    public override void StartHolding(ItemActionData _actionData)
    {
        var actionData = _actionData as ItemActionDataSpawnEletricalComponent;

        if (actionData.componentInstance != null)
            actionData.componentInstance.Cleanup();

        actionData.componentInstance = ElectricalComponentInstance.Create(item);

        UpdatePreview(actionData, true);
    }

    private void UpdatePreview(ItemActionDataSpawnEletricalComponent actionData, bool updateRotation)
    {
        if (actionData.componentInstance == null)
            return;

        var world = actionData.invData.world;
        var lookRay = actionData.invData.holdingEntity.GetLookRay();

        if (!Voxel.Raycast(world, lookRay, 10f, 8454144, 69, 0f))
        {
            actionData.componentInstance.IsActive = false;
            return;
        }

        var hitInfo = Voxel.voxelRayHitInfo;
        var hitPosition = hitInfo.hit.pos;
        var faceNormal = RayCastUtils.GetFaceNormal(hitInfo);

        if (updateRotation || actionData.blockFace != hitInfo.hit.blockFace)
            actionData.componentInstance.Rotation = Quaternion.FromToRotation(Vector3.up, faceNormal);

        TryRotatePreview(actionData);

        actionData.blockFace = hitInfo.hit.blockFace;
        actionData.componentInstance.Position = hitPosition;
        actionData.componentInstance.IsActive = true;
    }

    private void TryRotatePreview(ItemActionDataSpawnEletricalComponent actionData)
    {
        if (Input.GetKey(KeyCode.R) && Time.time - actionData.lastRotationTime > rotationDelay)
        {
            actionData.componentInstance.Rotate(Vector3.up, 90f);
            actionData.lastRotationTime = Time.time;
        }
    }

    public override void StopHolding(ItemActionData _actionData)
    {
        var actionData = _actionData as ItemActionDataSpawnEletricalComponent;
        if (actionData.componentInstance != null && actionData.invData.holdingEntity is EntityPlayerLocal)
        {
            actionData.componentInstance.Cleanup();
        }
    }

    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        if (!(_actionData is ItemActionDataSpawnEletricalComponent actionData))
            return;

        UpdatePreview(actionData, false);
    }

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        if (!_bReleased || Time.time - _actionData.lastUseTime < Delay)
            return;

        if (!(_actionData is ItemActionDataSpawnEletricalComponent actionData))
            return;

        if (actionData.componentInstance == null || !actionData.componentInstance.IsActive)
            return;

        ElectricalComponentManager.Instance.SpawnComponent(actionData.componentInstance.Clone());

        actionData.invData.itemStack.count--;
        actionData.invData.Changed();

        if (actionData.invData.itemStack.count == 0)
            StopHolding(actionData);
    }

}
