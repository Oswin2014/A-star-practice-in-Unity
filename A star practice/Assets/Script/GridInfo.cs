using UnityEngine;
using System.Collections;

public class GridInfo {

    GridModel model;

    protected int mTx, mTz;

    public int tx
    { get { return mTx; } }

    public int tz
    { get { return mTz; } }

    Vector3 mCenterPosition = Vector3.zero;

    public Vector3 centerPosition
    {
        get { return mCenterPosition; }
    }

    public GridInfo(GridModel model, int x, int z)
    {
        this.model = model;
        mTx = x;
        mTz = z;

        mCenterPosition.x = mTx * model.quadSize + model.quadSize * 0.5f;
        mCenterPosition.z = mTz * model.quadSize + model.quadSize * 0.5f;
    }

}
