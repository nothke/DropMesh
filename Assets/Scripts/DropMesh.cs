using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class DropMesh : MonoBehaviour
{

    TMesh tmesh;

    void Start()
    {
        int width = 3;

        Vector3[] points = new Vector3[width * width];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                points[y * width + x] = new Vector3(x, 0, y);
            }
        }

        TMesh tmesh = Simplify(points, width);
        gameObject.InitializeMesh(tmesh.ToMesh());
    }

    TMesh Simplify(Vector3[] vertices, int width)
    {
        List<int> tris = new List<int>();

        // in the first column we will drop one
        for (int x = 0; x < width - 1; x++)
        {
            int firstRowIncrement = 1;
            int secondRowIncrement = 1;

            for (int y = 0; y < width / 2; y += 2)
            {
                // first row
                int a = x * width + y;
                int b = x * width + y + 1;
                int c = x * width + y + 2;

                // second row
                int d = (x + 1) * width + y;
                int e = (x + 1) * width + y + 2; // here is the skipped one

                TriangleFan(ref tris, a, b, c, d, e); // we add triangles to the list
            }
        }

        TMesh tmesh = new TMesh();
        tmesh.vertices = vertices;
        tmesh.triangles = tris.ToArray();
        return tmesh;
    }

    void TriangleFan(ref List<int> tris, int a, int b, int c, int d)
    {
        tris.Add(a); tris.Add(c); tris.Add(b);
        tris.Add(b); tris.Add(c); tris.Add(d);
    }

    void TriangleFan(ref List<int> tris, int a, int b, int c, int d, int e)
    {
        Debug.Log(string.Format("Making tris from: {0}, {1}, {2}, {3}, {4}", a, b, c, d, e));

        tris.Add(a); tris.Add(d); tris.Add(b);
        tris.Add(b); tris.Add(d); tris.Add(e);
        tris.Add(b); tris.Add(e); tris.Add(c);
    }
}
