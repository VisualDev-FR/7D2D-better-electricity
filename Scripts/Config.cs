using UnityEngine;


public class Config
{
    private static readonly ModConfig modConfig = new ModConfig(version: 0, save: false);

    public static string ModPath => modConfig.modPath;

    public static Color wireDefaultColor = Color.black;

    public static Color wirePreviewColor = new Color(0f, 255f, 0f, 0.2f);

    public static Color nodePreviewColor = new Color(255f, 255f, 255f, 0.05f);

    public static int wireSegments = 6;

    public static float wireRadius = 0.015f;

    public static float wireOffset = 0.010f;

    public static Vector3 nodeScale = new Vector3(0.05f, 0.05f, 0.05f);

    public static Material MaterialSpritesDefault => new Material(Shader.Find("Sprites/Default"));
}