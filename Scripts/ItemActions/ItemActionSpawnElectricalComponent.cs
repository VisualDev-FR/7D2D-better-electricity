# pragma warning disable IDE0019, IDE0031

using System.Collections.Generic;
using UnityEngine;


public class ItemActionSpawnElectricalComponent : ItemAction
{
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

        if (!(actionData.invData.holdingEntity is EntityPlayerLocal entityPlayerLocal))
            return;

        if (actionData.transform != null)
            Object.DestroyImmediate(actionData.transform.gameObject);


        GameObject original = DataLoader.LoadAsset<GameObject>(entityPlayerLocal.inventory.holdingItem.MeshFile);

        actionData.transform = Object.Instantiate(original).transform;
        actionData.transform.gameObject.SetActive(false);

        UpdatePreview(actionData, true);
        SetConnectionsActive(actionData, false);
    }

    private void SetConnectionsActive(ItemActionDataSpawnEletricalComponent actionData, bool visible)
    {
        var connectorsTransform = UnityUtils.FindByName(actionData.transform, propConnectors);

        if (connectorsTransform != null)
        {
            connectorsTransform.gameObject.SetActive(visible);
        }
    }

    private void UpdatePreview(ItemActionDataSpawnEletricalComponent actionData, bool updateRotation)
    {
        if (actionData.transform == null || actionData.transform.gameObject == null)
            return;

        var world = actionData.invData.world;
        var lookRay = actionData.invData.holdingEntity.GetLookRay();

        if (!Voxel.Raycast(world, lookRay, 10f, 8454144, 69, 0f))
        {
            actionData.transform.gameObject.SetActive(false);
            return;
        }

        var hitInfo = Voxel.voxelRayHitInfo;
        var hitPosition = hitInfo.hit.pos;
        var faceNormal = RayCastUtils.GetFaceNormal(hitInfo);

        if (updateRotation || actionData.blockFace != hitInfo.hit.blockFace)
            actionData.transform.rotation = Quaternion.FromToRotation(Vector3.up, faceNormal);

        TryRotatePreview(actionData);

        actionData.transform.position = hitPosition;
        actionData.transform.gameObject.SetActive(true);
        actionData.blockFace = hitInfo.hit.blockFace;
    }

    private void TryRotatePreview(ItemActionDataSpawnEletricalComponent actionData)
    {
        if (Input.GetKey(KeyCode.R) && Time.time - actionData.lastRotation > rotationDelay)
        {
            actionData.transform.Rotate(Vector3.up, 90f);
            actionData.lastRotation = Time.time;
        }
    }

    public override void StopHolding(ItemActionData _actionData)
    {
        ItemActionDataSpawnEletricalComponent itemActionDataSpawnTurret = (ItemActionDataSpawnEletricalComponent)_actionData;
        if (itemActionDataSpawnTurret.transform != null && itemActionDataSpawnTurret.invData.holdingEntity is EntityPlayerLocal)
        {
            Object.Destroy(itemActionDataSpawnTurret.transform.gameObject);
        }
    }

    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        UpdatePreview(_actionData as ItemActionDataSpawnEletricalComponent, false);
    }

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        if (!_bReleased || Time.time - _actionData.lastUseTime < Delay)
            return;

        if (!(_actionData is ItemActionDataSpawnEletricalComponent actionData))
            return;

        if (actionData.transform == null || actionData.transform.gameObject == null)
            return;

        var gameObject = Object.Instantiate(actionData.transform.gameObject);
        var component = new ElectricalComponent(gameObject.transform);

        ElectricalComponentManager.Instance.AddComponentToWorld(component);

        actionData.invData.itemStack.count--;
        actionData.invData.Changed();
    }

}
