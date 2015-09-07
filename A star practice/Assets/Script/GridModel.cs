using UnityEngine;
using System.Collections;

public class GridModel : MonoBehaviour
{
    
#if UNITY_EDITOR

    GridView view;
#endif


    public static byte[,] grid;

    public static GridInfo[,] gridArray;

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    public int row;

    public int column;

    public float quadSize = 1.0f; 


#if UNITY_EDITOR
    protected void OnValidate()
    {
        init();

        view = cachedGameObject.GetComponent<GridView>();
        if (null == view)
            view = cachedGameObject.AddComponent<GridView>();

        //view.drawGrid();
    }
#endif

    public void init()
    {
        grid = new byte[row, column];
        gridArray = new GridInfo[row, column];

        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
            {
                GridInfo info = new GridInfo(this, i, j);
                gridArray[i, j] = info;
            }

        
    }

    public Vector2 getXZ(Vector3 pos)
    {
        Vector3 localPos = cachedTransform.worldToLocalMatrix.MultiplyPoint(pos);
        Vector2 xz = new Vector2(localPos.x / quadSize, localPos.z / quadSize);

        return xz;
    }

    public void setBlock(Vector3 pos, int data)
    {
        Vector2 xz = getXZ(pos);
        int x = Mathf.FloorToInt(xz.x);
        int z = Mathf.FloorToInt(xz.y);

        if (x >= row || z >= column)
            return;

        grid[x, z] = (byte)data;
    }
}
