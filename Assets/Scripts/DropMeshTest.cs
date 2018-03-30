using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class DropMeshTest : MonoBehaviour
{
    public int width = 8;
    public int length = 8;

    public bool[] ratios;

    Mesh mesh;

    void Update()
    {
        if (mesh == null) mesh = new Mesh();
        else Destroy(mesh);

        Vector3[] points = new Vector3[width * length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                float height = Mathf.PerlinNoise(x * 0.234f, y * 0.234f);
                points[x * length + y] = new Vector3(x, height, y);
            }
        }

        //bool[] ratios = new bool[] { false, false, true, true, false};
        DropMesh.ratios = ratios;
        TMesh tmesh = DropMesh.Get(points, width, length);
        mesh = tmesh.ToMesh();
        mesh.RecalculateNormals();
        gameObject.InitializeMesh(mesh);
    }
}
