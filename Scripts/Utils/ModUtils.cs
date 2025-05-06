using System;

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
}