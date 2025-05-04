using System.Collections.Generic;
using UnityEngine;

public static class RayCastUtils
{
    private static readonly Dictionary<BlockFace, Vector3> faceToNormal = new Dictionary<BlockFace, Vector3>()
    {
        { BlockFace.East,   new Vector3( 1,  0,  0)},
        { BlockFace.West,   new Vector3(-1,  0,  0)},
        { BlockFace.Top,    new Vector3( 0,  1,  0)},
        { BlockFace.Bottom, new Vector3( 0, -1,  0)},
        { BlockFace.North,  new Vector3( 0,  0,  1)},
        { BlockFace.South,  new Vector3( 0,  0, -1)},
    };

    public static Vector3 GetFaceNormal(WorldRayHitInfo hitInfo)
    {
        return faceToNormal[hitInfo.hit.blockFace];
    }

    public static Vector3 CalcHitPos(WorldRayHitInfo hitInfo, float offset = 0)
    {
        if (!hitInfo.bHitValid)
        {
            return Vector3.zero;
        }

        return hitInfo.hit.pos + faceToNormal[hitInfo.hit.blockFace] * offset;
    }

    public static Collider GetLookedAtCollider(EntityPlayer player, float distance = 4f)
    {
        if (Physics.Raycast(player.GetLookRay(), out var hitInfo, distance))
        {
            return hitInfo.collider;
        }

        return null;
    }

}