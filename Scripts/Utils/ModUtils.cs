using System;
using System.Linq;
using UnityEngine;

public class ModUtils
{
    public static void UIDenied(string key, string soundProp = "ui_denied")
    {
        var player = GameManager.Instance.World.GetPrimaryPlayer();

        GameManager.ShowTooltip(player, Localization.Get(key), string.Empty, soundProp);
    }

    public static T ParseEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static Vector3 ParseVector3(string value)
    {
        if (value == null)
            return Vector3.zero;

        var coords = value.Split(',')
            .Select(c => c.Trim())
            .ToArray();

        return new Vector3(
            float.Parse(coords[0]),
            float.Parse(coords[1]),
            float.Parse(coords[2])
        );
    }
}