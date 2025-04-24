using UnityEngine;


public class Config
{
    private static readonly ModConfig modConfig = new ModConfig(version: 0, save: false);

    public static string ModPath => modConfig.modPath;

    public static Color wireColor = new Color(0f, 255f, 0f, 0.2f);

    public static int wireSegments = 9;

    public static float wireRadius = 0.015f;
}