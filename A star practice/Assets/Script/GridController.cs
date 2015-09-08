
#if UNITY_EDITOR

using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour {

    GridModel model;

    GridView view;

    public bool full;

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }


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

        view = cachedGameObject.GetComponent<GridView>();
        if (null == view)
            view = cachedGameObject.AddComponent<GridView>();
    }

    public void setBlock(Vector3 pos, int data)
    {
        model.setBlock(pos, data);
        view.drawGrid();
    }


}

#endif