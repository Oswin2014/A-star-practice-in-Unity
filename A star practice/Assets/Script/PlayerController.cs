
//#define Draw_Calc_Node

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class PlayerController : MonoBehaviour {

    public static float costMilliseconds = 0f;

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

    public HeuristicFormula pathFindFormula = HeuristicFormula.Manhattan;

    public bool tieBreaker = false;

    public bool punishChangeDirection = false;

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

        pathFinder.formula = pathFindFormula;
        pathFinder.tieBreaker = tieBreaker;
        pathFinder.punishChangeDirection = punishChangeDirection;
    }

    void LateUpdate()
    {

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = mSceneCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitInfo, 50000, GenericHelper.GetLayerMask("Ground")))
            {
                mDestination = hitInfo.point;
                mDestination.y = cachedTransform.position.y;

                var sw = new Stopwatch();
                sw.Start();

#if Draw_Calc_Node
                Dictionary<int, PathNode> calcNode = new Dictionary<int, PathNode>();
                mPath = pathFinder.FindPath(cachedTransform.position, mDestination, ref calcNode);

#if UNITY_EDITOR
                gridModel.drawCalcNode(calcNode);
#endif

#else
                mPath = pathFinder.FindPath(cachedTransform.position, mDestination);
#endif

                sw.Stop();

                //if open define Draw_Calc_Node, the cost time is greater than truth.
                costMilliseconds += sw.ElapsedTicks / 10000.0f;

                if (null != mPath)
                {
                    mPath.Reverse();
#if UNITY_EDITOR
                    gridModel.drawPath(mPath);
#endif
                }

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
