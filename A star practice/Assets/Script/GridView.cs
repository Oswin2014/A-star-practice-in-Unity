﻿using UnityEngine;
using System.Collections;

public class GridView : MonoBehaviour {

    GridModel model;

    public float lineHalfWidth = 0.03f;

    public static Vector3 cubeSize = new Vector3(0.5f, 0.05f, 0.5f);

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    Mesh gridMesh;

    // Use this for initialization
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        init();
    }
#endif


    public void init()
    {

        model = cachedGameObject.GetComponent<GridModel>();
        if (null == model)
            model = cachedGameObject.AddComponent<GridModel>();

        MeshFilter meshFilter = cachedGameObject.GetComponent<MeshFilter>();

        if (null == meshFilter)
            meshFilter = cachedGameObject.AddComponent<MeshFilter>();

        gridMesh = meshFilter.sharedMesh;

        if (null == cachedGameObject.GetComponent<MeshRenderer>())
            cachedGameObject.AddComponent<MeshRenderer>();

        drawGrid();
    }

    public void drawGrid()
    {
        int row = model.row, column = model.column;
        float lineSpan = model.quadSize;

        DrawMeshData meshData = new DrawMeshData();
        int lineVertexCount = (row + column + 2) * 4;

        int cubeCount = 0;
        for(int i = 0; i < row; i++)
            for(int j = 0; j < column; j++)
            {
                if (1 == GridModel.grid[i, j])
                    cubeCount++;
            }
        

        int vertexCount = lineVertexCount + cubeCount * 8;
        int triangleCount = lineVertexCount / 2 * 3 + cubeCount * 12 * 3;

        meshData.vertices = new Vector3[vertexCount];
        meshData.colors = new Color[vertexCount];
        meshData.triangles = new int[triangleCount];

        //draw grid
        Vector3 rowBeginPoint = Vector3.zero, colBeginPoint = Vector3.zero;
        for (int i = 0; i <= row; i++)
        {
            drawLine(ref meshData, rowBeginPoint, rowBeginPoint + Vector3.right * column * lineSpan, lineHalfWidth, Vector3.back);
            rowBeginPoint += Vector3.forward * lineSpan;
        }

        for (int j = 0; j <= column; j++)
        {
            drawLine(ref meshData, colBeginPoint, colBeginPoint + Vector3.forward * row * lineSpan, lineHalfWidth, Vector3.right);
            colBeginPoint += Vector3.right * lineSpan;
        }

        //draw block cube
        //drawCube(ref meshData, new Vector3(0.5f, 0, 0.5f), cubeSize);

        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
            {
                if (1 == GridModel.grid[i, j])
                {
                    drawCube(ref meshData, GridModel.gridArray[i,j].centerPosition, cubeSize);
                }
            }

        gridMesh.triangles = null;
        gridMesh.vertices = meshData.vertices;
        gridMesh.colors = meshData.colors;
        gridMesh.SetTriangles(meshData.triangles, 0);
    }

    public void drawLine(ref DrawMeshData meshData, Vector3 beginPoint, Vector3 endPoint, float lineHalfWidth, Vector3 zDir)
    {
        Color color = Color.blue;

        Vector3 offset = zDir.normalized * lineHalfWidth;
        Vector3 tl = beginPoint + offset;
        Vector3 bl = beginPoint - offset;
        Vector3 br = endPoint - offset;
        Vector3 tr = endPoint + offset;

        int tlIndex = 0, blIndex = 0, brIndex = 0, trIndex = 0;

        meshData.vertices[meshData.vexIndex] = tl;
        meshData.colors[meshData.vexIndex] = color;
        tlIndex = meshData.vexIndex++;

        meshData.vertices[meshData.vexIndex] = bl;
        meshData.colors[meshData.vexIndex] = color;
        blIndex = meshData.vexIndex++;

        meshData.vertices[meshData.vexIndex] = br;
        meshData.colors[meshData.vexIndex] = color;
        brIndex = meshData.vexIndex++;

        meshData.vertices[meshData.vexIndex] = tr;
        meshData.colors[meshData.vexIndex] = color;
        trIndex = meshData.vexIndex++;

        meshData.triangles[meshData.triIndex++] = tlIndex;
        meshData.triangles[meshData.triIndex++] = blIndex;
        meshData.triangles[meshData.triIndex++] = brIndex;

        meshData.triangles[meshData.triIndex++] = brIndex;
        meshData.triangles[meshData.triIndex++] = trIndex;
        meshData.triangles[meshData.triIndex++] = tlIndex;
    }

    public void drawCube(ref DrawMeshData meshData, Vector3 center, Vector3 size)
    {
        Color color = Color.red;

        Vector3[] dir = new Vector3[8] 
        { 
            //left-bottom-front
            new Vector3(-1,-1, 1),
            //right-bottom-front
            new Vector3( 1,-1, 1),
            //right-bottom-back
            new Vector3( 1,-1,-1),
            //left-bottom-back
            new Vector3(-1,-1,-1),
            //left-top-front
            new Vector3(-1, 1, 1),
            //right-top-front 
            new Vector3( 1, 1, 1),
            //right-top-back 
            new Vector3( 1, 1,-1),
            //left-top-back
            new Vector3(-1, 1,-1),
        };

        int[] indexs = new int[8];

        for (int i = 0; i < 8; i++)
        {
            Vector3 vertex = center + Vector3.Scale(size, dir[i]);

            meshData.vertices[meshData.vexIndex] = vertex;
            meshData.colors[meshData.vexIndex] = color;
            indexs[i] = meshData.vexIndex++;
        }

        int[,] triangles = new int[12, 3] 
        { 
            //left
            {0,4,7},
            {7,3,0},
                             
            //right          
            {6,5,1},
            {1,2,6},
                             
            //bottom         
            {2,1,0},
            {0,3,5},
                             
            //top            
            {4,5,6},
            {6,7,4},
                             
            //back           
            {7,6,2},
            {2,3,7},
            
            //front
            {5,4,0},
            {0,1,5}
        };

        for (int i = 0; i < 11; i++)
            for (int j = 0; j < 3; j++)
            {
                meshData.triangles[meshData.triIndex++] = indexs[triangles[i, j]];
            }
    }


}