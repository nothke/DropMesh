using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class DropMesh : MonoBehaviour
{

    TMesh tmesh;

    public int width = 8;

    void Start()
    {
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

        int firstRowIncrement = 1;
        int secondRowIncrement = 2;

        int a, b, c, d, e;

        for (int x = 0; x < width - 1; x++)
        {
            int y = 0;

            bool incomplete = false;

            while (true)
            {
                int toEnd = width - y - 1;

                // Ending
                if (toEnd <= firstRowIncrement)
                {
                    Debug.Log(incomplete ? "Incomplete" : "Final");

                    if (!incomplete) // in case the row ends with a quad
                    {
                        a = x * width + y;
                        b = x * width + width - 1;

                        c = (x + 1) * width + y;
                        d = (x + 1) * width + width - 1;

                        Triangulate(ref tris, a, b, c, d);
                    }
                    else // in case the row ends with a triangle
                    {
                        Debug.Log(y);

                        a = x * width + y;
                        b = x * width + width - 1;
                        c = (x + 1) * width + width - 1;

                        Debug.Log(string.Format("ABC: {0}, {1}, {2}", a, b, c));
                        tris.Add(b); tris.Add(a); tris.Add(c);
                    }

                    break;
                }

                // first row
                a = x * width + y;
                b = x * width + y + firstRowIncrement;
                c = x * width + y + firstRowIncrement * 2;

                // second row
                d = (x + 1) * width + y;
                e = (x + 1) * width + y + secondRowIncrement; // here is the skipped one

                if (y + secondRowIncrement >= width) e = (x + 1) * width + width - 1;
                if (c != e - width) incomplete = true;

                Triangulate(ref tris, a, b, c, d, e); // we add triangles to the list

                y += secondRowIncrement;
            }

            Debug.Log("Row END");

            if (x < 2)
            {

                firstRowIncrement = secondRowIncrement;
                secondRowIncrement = firstRowIncrement * 2;
            }
        }

        TMesh tmesh = new TMesh();
        tmesh.vertices = vertices;
        tmesh.triangles = tris.ToArray();
        return tmesh;
    }

    void Triangulate(ref List<int> tris, int a, int b, int c, int d)
    {
        tris.Add(a); tris.Add(c); tris.Add(b);
        tris.Add(b); tris.Add(c); tris.Add(d);
    }

    void Triangulate(ref List<int> tris, int a, int b, int c, int d, int e)
    {
        Debug.Log(string.Format("Making tris from: {0}, {1}, {2}, {3}, {4}", a, b, c, d, e));

        tris.Add(a); tris.Add(d); tris.Add(b);
        tris.Add(b); tris.Add(d); tris.Add(e);
        tris.Add(b); tris.Add(e); tris.Add(c);
    }
}
