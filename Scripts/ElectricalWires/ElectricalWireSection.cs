using UnityEngine;


public class ElectricalWireSection : MonoBehaviour
{
    public const string propGameObject = "ElectricalWireSection";

    private static int SegmentsCount => Config.wireSegments;

    private static float Radius => Config.wireRadius;

    public ElectricalWire Parent { get; private set; }

    public Vector3 StartPos { get; private set; }

    public Vector3 EndPos { get; private set; }

    private MeshRenderer meshRenderer;

    private BoxCollider collider;

    public Color Color
    {
        get => meshRenderer.material.color;
        set => meshRenderer.material.color = value;
    }

    public bool IsActive
    {
        get => transform.gameObject.activeSelf;
        set => transform.gameObject.SetActive(value);
    }

    public static ElectricalWireSection Create(ElectricalWire parent, Vector3 start, Vector3 end)
    {
        var wireSection = new GameObject(propGameObject).AddComponent<ElectricalWireSection>();

        wireSection.transform.parent = parent.transform;
        wireSection.StartPos = start;
        wireSection.EndPos = end;
        wireSection.Parent = parent;

        return wireSection;
    }

    public void Start()
    {
        var direction = EndPos - StartPos;
        var length = direction.magnitude;
        var colliderWidth = Config.wireRadius * 4;

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = GenerateCylinderMesh(length);

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Config.MaterialSpritesDefault;
        meshRenderer.material.color = Config.wireDefaultColor;

        collider = gameObject.AddComponent<BoxCollider>();
        collider.center = new Vector3(0, length / 2, 0);
        collider.size = new Vector3(colliderWidth, length, colliderWidth);

        gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        gameObject.transform.position = StartPos;
    }

    private Mesh GenerateCylinderMesh(float length)
    {
        var mesh = new Mesh();
        var vertexCount = 2 * SegmentsCount + 2;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[SegmentsCount * 12];

        for (int i = 0; i < SegmentsCount; i++)
        {
            float angle = Mathf.PI * 2 * i / SegmentsCount;
            float x = Mathf.Cos(angle) * Radius;
            float z = Mathf.Sin(angle) * Radius;

            vertices[i] = new Vector3(x, 0, z);
            vertices[i + SegmentsCount] = new Vector3(x, length, z);
        }

        vertices[vertexCount - 2] = Vector3.zero;
        vertices[vertexCount - 1] = new Vector3(0, length, 0);


        int triIndex = 0;

        for (int i = 0; i < SegmentsCount; i++)
        {
            int next = (i + 1) % SegmentsCount;

            triangles[triIndex++] = i;
            triangles[triIndex++] = next;
            triangles[triIndex++] = i + SegmentsCount;

            triangles[triIndex++] = i + SegmentsCount;
            triangles[triIndex++] = next;
            triangles[triIndex++] = next + SegmentsCount;
        }

        for (int i = 0; i < SegmentsCount; i++)
        {
            int next = (i + 1) % SegmentsCount;

            triangles[triIndex++] = next;
            triangles[triIndex++] = i;
            triangles[triIndex++] = vertexCount - 2;

            triangles[triIndex++] = i + SegmentsCount;
            triangles[triIndex++] = next + SegmentsCount;
            triangles[triIndex++] = vertexCount - 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

}