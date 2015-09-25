using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

public class DrawMeshData
{

    public Vector3[] vertices;

    public Color[] colors;

    //index buf
    public int[] triangles;

    //note vertex write progress index
    public int vexIndex;

    //note index buf write progress index
    public int triIndex;

}

#endif
