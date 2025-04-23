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

}