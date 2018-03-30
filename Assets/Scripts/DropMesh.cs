using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMesh
{
    public static bool[] ratios;

    public static TMesh Get(Vector3[] vertices, int width, int length)
    {
        List<int> tris = new List<int>();
        List<Vector3> verts = new List<Vector3>();

        int rowIncrement = 1;

        for (int x = 0; x < width; x++)
        {
            rowIncrement = GetRowIncrement(x);

            for (int y = 0; y < length; y++)
            {
                if (y == (length - 1) || (y % rowIncrement) == 0)
                    verts.Add(vertices[x * length + y]);
            }
        }

        // TRIANGLES

        int a, b, c, d, e;

        int thisRowIncrement = GetRowIncrement(0);
        int nextRowIncrement;

        int thisRowCount = GetRowCount(length, thisRowIncrement);
        int nextRowCount;

        int thisRow0 = 0;
        int nextRow0;

        for (int x = 0; x < width - 1; x++)
        {
            nextRowIncrement = GetRowIncrement(x + 1);
            nextRowCount = GetRowCount(length, nextRowIncrement);
            nextRow0 = thisRow0 + thisRowCount;

            // Just output row counts for now
            Debug.Log(thisRowCount);
            Debug.Log("nextRow0 " + nextRow0);

            int thisRowV = thisRow0;
            int nextRowV = nextRow0;

            bool drop = thisRowIncrement != nextRowIncrement;

            if (drop)
            {
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
            }
            else
            {
                for (int y = 0; y < thisRowCount - 1; y++)
                {
                    //Debug.LogWarning("tR0: " + thisRow0);
                    //Debug.LogWarning("nR0: " + nextRow0);

                    a = thisRowV;
                    b = thisRowV + 1;
                    c = nextRowV;
                    d = nextRowV + 1;

                    //if (c >= verts.Count) Debug.LogError("a is " + a + "c is " + c);
                    //if (d >= verts.Count) Debug.LogError("b is " + b + "d is " + d);

                    Triangulate(ref tris, a, b, c, d);

                    thisRowV++;
                    nextRowV++;
                }
            }

            thisRowIncrement = nextRowIncrement;
            thisRowCount = nextRowCount;
            thisRow0 = nextRow0;
        }

        // Finalize
        TMesh tmesh = new TMesh();
        tmesh.vertices = verts.ToArray();
        tmesh.triangles = tris.ToArray();
        return tmesh;
    }

    static int GetRowIncrement(int row)
    {
        int pow = 0;

        if (ratios == null)
            pow = row;

        for (int i = 0; i < row; i++)
        {
            if (GetDropRatio(i))
                pow++;
        }

        return (int)Mathf.Pow(2, pow);
    }

    static bool GetDropRatio(int row)
    {
        if (ratios == null || ratios.Length == 0)
        {
            return true;
        }
        else
        {
            if (row < ratios.Length)
                return ratios[row];
            else
                return ratios[ratios.Length - 1];
        }
    }

    static int GetRowCount(int length, int increment)
    {
        int count = 0;

        for (int y = 0; y < length; y++)
            if (y == (length - 1) || (y % increment) == 0)
                count++;

        return count;
    }

    static void Triangulate(ref List<int> tris, int a, int b, int c, int d)
    {
        tris.Add(a); tris.Add(b); tris.Add(c);
        tris.Add(b); tris.Add(d); tris.Add(c);
    }

    static void Triangulate(ref List<int> tris, int a, int b, int c, int d, int e)
    {
        //Debug.Log(string.Format("Making tris from: {0}, {1}, {2}, {3}, {4}", a, b, c, d, e));

        tris.Add(a); tris.Add(b); tris.Add(d);
        tris.Add(b); tris.Add(e); tris.Add(d);
        tris.Add(b); tris.Add(c); tris.Add(e);
    }
}
