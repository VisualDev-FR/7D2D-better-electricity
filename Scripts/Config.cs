using UnityEngine;

public class Config
{
    private static readonly ModConfig modConfig = new ModConfig("BetterElectricity", version: 0, save: false);

    public static float lineWidth = 0.05f;

    public static Color lineColor = Color.black;
}