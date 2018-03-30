using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class DropMesh : MonoBehaviour
{

    TMesh tmesh;

    public int width = 8;
    public int length = 8;

    void Start()
    {
        Vector3[] points = new Vector3[width * length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                float height = 0;// Mathf.PerlinNoise(x * 0.234f, y * 0.234f);
                points[x * length + y] = new Vector3(x * x * 2, height, y);
            }
        }

        TMesh tmesh = Simplify2(points, width, length);
        Mesh mesh = tmesh.ToMesh();
        mesh.RecalculateNormals();
        gameObject.InitializeMesh(mesh);
    }

    TMesh Simplify2(Vector3[] vertices, int width, int length)
    {
        List<int> tris = new List<int>();
        List<Vector3> droppedVerts = new List<Vector3>();

        int rowIncrement = 1;

        for (int x = 0; x < width; x++)
        {
            rowIncrement = GetRowIncrement(x);

            for (int y = 0; y < length; y++)
            {
                if (y == (length - 1) || (y % rowIncrement) == 0)
                    droppedVerts.Add(vertices[x * length + y]);
            }
        }

        // Debug these dropped verts
        for (int i = 0; i < droppedVerts.Count; i++)
        {
            Debug.DrawRay(droppedVerts[i], Vector3.up, Color.red, 1);
        }

        // TRIANGLES

        int a, b, c, d, e;

        int thisRowIncrement = GetRowIncrement(0);
        int nextRowIncrement;

        int thisRowCount = GetRowCount(0, thisRowIncrement);
        int nextRowCount;

        int thisRow0 = 0;
        int nextRow0;

        for (int x = 0; x < width - 1; x++)
        {
            nextRowIncrement = GetRowIncrement(x + 1);
            nextRowCount = GetRowCount(x + 1, nextRowIncrement);
            nextRow0 = thisRow0 + thisRowCount;

            // Just output row counts for now
            Debug.Log(thisRowCount);
            Debug.Log("nextRow0 " + nextRow0);

            int thisRowV = thisRow0;
            int nextRowV = nextRow0;

            while (true)
            {
                a = thisRowV;
                b = thisRowV + 1;
                c = thisRowV + 2;

                d = nextRowV;
                e = nextRowV + 1;

                // End condition
                if (c >= nextRow0)
                {
                    // If the piece cannot be dropped, make a quad
                    if (b - thisRow0 < thisRowCount)
                    {
                        Debug.Log("Quad situation");
                        Triangulate(ref tris, a, b, d, e);
                        break;
                    }
                    else
                        break;
                }

                Triangulate(ref tris, a, b, c, d, e);

                thisRowV += 2;
                nextRowV += 1;
            }

            thisRowIncrement = nextRowIncrement;
            thisRowCount = nextRowCount;
            thisRow0 = nextRow0;
        }

        // Finalize
        TMesh tmesh = new TMesh();
        tmesh.vertices = droppedVerts.ToArray();
        tmesh.triangles = tris.ToArray();
        return tmesh;
    }

    int GetRowIncrement(int row)
    {
        return (int)Mathf.Pow(2, row);
    }

    int GetRowCount(int row, int increment)
    {
        int count = 0;

        for (int y = 0; y < length; y++)
            if (y == (length - 1) || (y % increment) == 0)
                count++;

        return count;
    }

    TMesh Simplify(Vector3[] vertices, int width, int length)
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
                int toEnd = length - y - 1;

                // Ending
                if (toEnd <= firstRowIncrement)
                {
                    Debug.Log(incomplete ? "Incomplete" : "Final");

                    if (!incomplete) // in case the row ends with a quad
                    {
                        a = x * length + y;
                        b = x * length + length - 1;

                        c = (x + 1) * length + y;
                        d = (x + 1) * length + length - 1;

                        Triangulate(ref tris, a, b, c, d);
                    }
                    else // in case the row ends with a triangle
                    {
                        y = y - secondRowIncrement + firstRowIncrement;

                        Debug.Log(y);

                        a = x * length + y;
                        b = x * length + length - 1;
                        c = (x + 1) * length + length - 1;

                        Debug.Log(string.Format("ABC: {0}, {1}, {2}", a, b, c));
                        tris.Add(a); tris.Add(b); tris.Add(c);
                    }

                    break;
                }

                // first row
                a = x * length + y;
                b = x * length + y + firstRowIncrement;
                c = x * length + y + firstRowIncrement * 2;

                // second row
                d = (x + 1) * length + y;
                e = (x + 1) * length + y + secondRowIncrement; // here is the skipped one

                if (y + secondRowIncrement >= length) e = (x + 1) * length + length - 1;
                if (c != e - length) incomplete = true;

                Triangulate(ref tris, a, b, c, d, e); // we add triangles to the list

                y += secondRowIncrement;
            }

            Debug.Log("Row END");

            firstRowIncrement = secondRowIncrement;
            secondRowIncrement = firstRowIncrement * 2;

        }

        TMesh tmesh = new TMesh();
        tmesh.vertices = vertices;
        tmesh.triangles = tris.ToArray();
        return tmesh;
    }

    void Triangulate(ref List<int> tris, int a, int b, int c, int d)
    {
        tris.Add(a); tris.Add(b); tris.Add(c);
        tris.Add(b); tris.Add(d); tris.Add(c);
    }

    void Triangulate(ref List<int> tris, int a, int b, int c, int d, int e)
    {
        //Debug.Log(string.Format("Making tris from: {0}, {1}, {2}, {3}, {4}", a, b, c, d, e));

        tris.Add(a); tris.Add(b); tris.Add(d);
        tris.Add(b); tris.Add(e); tris.Add(d);
        tris.Add(b); tris.Add(c); tris.Add(e);
    }
}
