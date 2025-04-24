using UnityEngine;


public class WireBuilder
{
    private static int SegmentsCount => Config.wireSegments;

    private static float Radius => Config.wireRadius;

    public static GameObject BuildWire(Transform parent, Vector3 start, Vector3 end)
    {
        var cylinder = new GameObject("Cylinder");
        var direction = end - start;
        var length = direction.magnitude;

        var meshFilter = cylinder.AddComponent<MeshFilter>();
        meshFilter.mesh = GenerateCylinderMesh(length);

        var meshRenderer = cylinder.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.black;

        cylinder.transform.position = start;
        cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        cylinder.transform.parent = parent;

        return cylinder;
    }

    private static Mesh GenerateCylinderMesh(float length)
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