using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GridModel : MonoBehaviour
{

    public enum NodeState
    {
        Open,
        Block,
        DrawPath,
    }
    
#if UNITY_EDITOR

    GridView view;
#endif

    //[SerializeField]
    //public byte[,] grid;

    //[SerializeField]
    //public static GridInfo[,] gridArray;

    [SerializeField]
    List<byte> gridList = new List<byte>();

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    public int row, column;

    [SerializeField]
    [HideInInspector]
    int oldRow, oldColumn;

    public float quadSize = 1.0f; 

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

#if UNITY_EDITOR
    protected void OnValidate()
    {
        init();

        view = cachedGameObject.GetComponent<GridView>();
        if (null == view)
            view = cachedGameObject.AddComponent<GridView>();

        view.updateInfo();
    }

    public void drawPath(List<PathNode> pathList)
    {
        byte drawFlag = (byte)NodeState.DrawPath;
        byte openFlag = (byte)NodeState.Open;
        for(int i = 0, max = gridList.Count; i < max; i++)
        {
            if (gridList[i] == drawFlag)
                gridList[i] = openFlag;
        }

        PathNode node;
        for(int i = 0, max = pathList.Count; i < max; i++)
        {
            node = pathList[i];
            gridList[node.X * column + node.Y] = drawFlag;
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

        return pos;
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
