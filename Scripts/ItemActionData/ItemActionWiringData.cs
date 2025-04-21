
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemActionWiringData : ItemActionAttackData
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<ItemActionWiringData>();

    public readonly List<Vector3> points = new List<Vector3>();

    private LineRenderer LineRenderer
    {
        get
        {
            var line = PlayerUI.GetComponent<LineRenderer>();

            if (line is null)
            {
                line = PlayerUI.gameObject.AddComponent<LineRenderer>();
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.receiveShadows = false;

                line.positionCount = 1;
                line.startWidth = Config.lineWidth;
                line.endWidth = Config.lineWidth;
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.startColor = Config.lineColor;
                line.endColor = Config.lineColor;
            }

            return line;
        }
    }

    public bool IsWiring { get; set; }

    public bool HasStartPoint { get; set; }

    public LocalPlayerUI PlayerUI { get; set; }

    public Vector3 StartPoint => points[0];

    public bool inRange;

    public bool isFriendly;

    public WireNode wireNode;

    public ItemActionWiringData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction) { }

    public WorldRayHitInfo GetHitInfo()
    {
        var hitInfo = invData.hitInfo;

        if (hitInfo.bHitValid)
        {
            return hitInfo;
        }

        return null;
    }

    public void StartWiring()
    {
        IsWiring = true;
        lastUseTime = Time.time;
    }

    public void StopWiring()
    {
        IsWiring = false;
        LineRenderer.SetPositions(Array.Empty<Vector3>());
    }

    public bool TryAddPoint()
    {
        var hitInfo = GetHitInfo();

        if (hitInfo is null)
            return false;

        points.Add(hitInfo.hit.pos);

        LineRenderer.positionCount += 1;
        LineRenderer.SetPosition(LineRenderer.positionCount - 2, hitInfo.hit.pos);

        for (int i = 0; i < LineRenderer.positionCount; i++)
        {
            logger.Debug(LineRenderer.GetPosition(i));
        }

        UpdateHitPos();

        return true;
    }

    public void UpdateHitPos()
    {
        var hitInfo = GetHitInfo();

        if (hitInfo != null)
        {
            LineRenderer.SetPosition(LineRenderer.positionCount - 1, hitInfo.hit.pos);
        }
    }

    public void ClearWires()
    {
        points.Clear();
        LineRenderer.positionCount = 1;
    }

    public void RemoveLastPoint()
    {
        logger.Debug($"RemoveLastPoint, count: {points.Count}");

        if (points.Count == 0)
            return;

        points.RemoveAt(points.Count - 1);

        LineRenderer.positionCount = points.Count;
        LineRenderer.SetPositions(points.ToArray());
        LineRenderer.positionCount++;

        UpdateHitPos();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
