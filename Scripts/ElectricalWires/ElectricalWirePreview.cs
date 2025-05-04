using UnityEngine;


public class ElectricalWirePreview
{
    private LineRenderer line;

    private GameObject sphere;

    public Vector3 Start
    {
        get => line.GetPosition(0);
        set => line.SetPosition(0, value);
    }

    public Vector3 End
    {
        get => line.GetPosition(1);
        set => line.SetPosition(1, value);
    }

    public ElectricalWirePreview(EntityPlayer player)
    {
        line = player.transform.GetComponent<LineRenderer>();

        if (line != null)
            return;

        var gameObject = new GameObject();

        line = gameObject.AddComponent<LineRenderer>();
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;

        line.positionCount = 2;
        line.startWidth = Config.wireRadius;
        line.endWidth = Config.wireRadius;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Config.wirePreviewColor;
        line.endColor = Config.wirePreviewColor;

        sphere = CreateSphere(0.02f);
        sphere.transform.parent = gameObject.transform;
    }

    private GameObject CreateSphere(float radius)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(radius, radius, radius);
        Object.Destroy(sphere.GetComponent<Collider>());

        var renderer = sphere.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.material.color = Config.wirePreviewColor;

        return sphere;
    }

    public void Cleanup()
    {
        Object.Destroy(line);

        line = null;
        sphere = null;
    }

    public void SetActive(bool value)
    {
        if (line.gameObject.activeSelf != value)
        {
            line.gameObject.SetActive(value);
        }
    }

    public void Update(WorldRayHitInfo hitInfo)
    {
        if (hitInfo.bHitValid)
        {
            End = RayCastUtils.CalcHitPos(hitInfo);
            sphere.transform.position = End;
            SetActive(true);
        }
        else
        {
            SetActive(false);
        }
    }
}