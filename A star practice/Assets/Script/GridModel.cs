using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GridModel : MonoBehaviour
{

    
#if UNITY_EDITOR

    public GridView view;
#endif

    //[SerializeField]
    //public byte[,] grid;

    //[SerializeField]
    //public static GridInfo[,] gridArray;

    [SerializeField]
    List<byte> gridList = new List<byte>();

    List<int> drawPathList = new List<int>();

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    public int row, column;

    [SerializeField]
    [HideInInspector]
    int oldRow, oldColumn;

    public float quadSize = 1.0f;

    public bool drawPathGrid = true;

    void Awake()
    {

        //gridArray = new GridInfo[row, column];
        //
        //for (int i = 0; i < row; i++)
        //    for (int j = 0; j < column; j++)
        //    {
        //        GridInfo info = new GridInfo(this, i, j);
        //        gridArray[i, j] = info;
        //    }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clearDrawPath();
        }
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        init();

        view = cachedGameObject.GetComponent<GridView>();
        if (null == view)
            view = cachedGameObject.AddComponent<GridView>();

        view.updateInfo();
    }

    void clearDrawPath()
    {
        for (int i = 0, max = drawPathList.Count; i < max; i++)
        {
            gridList[drawPathList[i]] = NodeState.Open;
        }
        drawPathList.Clear();
    }

    public void drawPath(List<PathNode> pathList)
    {
        if (!drawPathGrid)
            return;

        PathNode node;
        int index = 0;
        for(int i = 0, max = pathList.Count; i < max; i++)
        {
            node = pathList[i];
            index = node.X * column + node.Y;
            byte nodeDate = gridList[index];
            if (nodeDate != NodeState.Block && nodeDate != NodeState.DrawPath)
            {
                gridList[index] = NodeState.DrawPath;
                drawPathList.Add(index);
            }
        }

        view.updateInfo();
    }

#endif

    public void init()
    {
        if (oldRow == row && oldColumn == column)
            return;

        //grid = new byte[row, column];

        List<byte> newGridList = new List<byte>(row*column);

        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
            {
                if (i < oldRow && j < oldColumn)
                    newGridList.Add(gridList[i * oldColumn + j]);
                else
                    newGridList.Add(0);
            }

        gridList = newGridList;

        oldRow = row;
        oldColumn = column;
    }

    public byte getTargetGrid(int x, int z)
    {
        return gridList[x * column + z];
        //return grid[x,z];
    }

    public Vector3 getGridCenterPos(int x, int z)
    {
        Vector3 pos = Vector3.zero;
        pos.x = x * quadSize + quadSize * 0.5f;
        pos.z = z * quadSize + quadSize * 0.5f;

        return cachedTransform.localToWorldMatrix.MultiplyPoint(pos);
    }

    public Point getXZ(Vector3 pos)
    {
        Vector3 localPos = cachedTransform.worldToLocalMatrix.MultiplyPoint(pos);
        Point xz = new Point(Mathf.FloorToInt(localPos.x / quadSize), Mathf.FloorToInt(localPos.z / quadSize));

        return xz;
    }

    public void setBlock(Vector3 pos, int data)
    {
        Point xz = getXZ(pos);
        int x = xz.x;
        int z = xz.y;

        if (x < 0 || z < 0 || x >= row || z >= column)
            return;

        byte value = (byte)data;
        //grid[x, z] = value;

        //Debug.Log("x: "+x+" z: "+z);
        gridList[x * column + z] = value;
    }



}
