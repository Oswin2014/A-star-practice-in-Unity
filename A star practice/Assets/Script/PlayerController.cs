
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class PlayerController : MonoBehaviour {

    public static long costMilliseconds = 0;

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    Camera mSceneCamera;

    Vector3 mDestination;

    GridModel gridModel;

    PathFinder pathFinder;

    List<PathNode> mPath;

    public float speed = 12;

    void Awake()
    {
        mSceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        mDestination = cachedTransform.position;

        gridModel = GameObject.Find("GridArea").GetComponent<GridModel>();

        pathFinder = new PathFinder(gridModel);
    }

    void Update()
    {
        costMilliseconds = 0;
    }

    void LateUpdate()
    {

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = mSceneCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitInfo, 500, GenericHelper.GetLayerMask("Ground")))
            {
                mDestination = hitInfo.point;
                mDestination.y = cachedTransform.position.y;

                var sw = new Stopwatch();
                sw.Start();

                mPath = pathFinder.FindPath(cachedTransform.position, mDestination);
                if (null != mPath)
                    mPath.Reverse();

                sw.Stop();
                costMilliseconds += sw.ElapsedMilliseconds;
                //UnityEngine.Debug.Log("cost:  " + sw.ElapsedMilliseconds);
                gridModel.view.updatePathFinderCost(costMilliseconds);
            }
        }


        if(null != mPath && !Mathf.Approximately(Vector3.Distance(cachedTransform.position, mDestination), 0f))
        {
            Vector3 goal = Vector3.zero;
            if (mPath.Count > 0)
            {
                PathNode node = mPath[0];
                goal = gridModel.getGridCenterPos(node.X, node.Y);
            }
            else
                goal = mDestination;

            goal.y = cachedTransform.position.y;
            cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, goal, speed * Time.deltaTime);

            if(Mathf.Approximately(Vector3.Distance(cachedTransform.position, goal), 0f))
            {
                if(mPath.Count > 0)
                    mPath.RemoveAt(0);

                if(0 == mPath.Count)
                    mPath = null;
            }
        }
    }



}
