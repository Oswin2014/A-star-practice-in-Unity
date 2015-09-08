
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    Camera mSceneCamera;

    Vector3 mDestination;

    GridModel gridModel;

    PathFinder pathFinder;

    public float speed = 6;

    void Awake()
    {
        mSceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        mDestination = cachedTransform.position;

        gridModel = GameObject.Find("GridArea").GetComponent<GridModel>();

        pathFinder = new PathFinder(gridModel);
    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = mSceneCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitInfo, 500, GenericHelper.GetLayerMask("Ground")))
            {
                mDestination = hitInfo.point;
                mDestination.y = cachedTransform.position.y;
                pathFinder.FindPath(cachedTransform.position, mDestination);
            }
        }

        cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, mDestination, speed * Time.deltaTime);
    }

}
