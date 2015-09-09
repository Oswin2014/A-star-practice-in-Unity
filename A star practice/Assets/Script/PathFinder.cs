using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathFinder {

    GridModel mModel;

    //byte[,] mGrid;

    PathNodeFast[] mCalcGrid;

    sbyte[,] mDirection = new sbyte[8, 2] {{0,-1} , {1,0}, {0,1}, {-1,0}, {1,-1}, {1,1}, {-1,1}, {-1,-1}};

    HeapTree mOpen;

    List<PathNode> mClose = new List<PathNode>();

    int mCloseNodeCounter;

    int mLocation;
    int mEndLocation;

    int mHEstimate = 2;

    ushort mGridX;
    ushort mGridY;

    ushort mGridXBitWide;
    ushort mGridXMinus;

    byte mOpenNodeValue = 1;
    byte mCloseNodeValue = 2;

    bool mFound;
    bool mStop;
    bool mStopped = true;

    bool mHeavyDiagonals = true;


    public PathFinder(GridModel model)
    {
        mModel = model;
        mGridX = (ushort)mModel.row;
        mGridY = (ushort)mModel.column;

        mGridXBitWide = (ushort)(Math.Log(mGridX - 1, 2) + 1);
        mGridXMinus = (ushort)( Math.Pow(2, mGridXBitWide) - 1);
        
        int maxIndex = ( (mGridY - 1) << mGridXBitWide ) + (mGridX - 1);
        //0 - maxIndex
        int size = maxIndex + 1;
        if (null == mCalcGrid || mCalcGrid.Length != (size))
            mCalcGrid = new PathNodeFast[size];

        mOpen = new HeapTree(mCalcGrid);
    }

    public List<PathNode> FindPath(Vector3 startPos, Vector3 endPos)
    {
        lock(this)
        {
            mFound = false;
            mStop = false;
            mStopped = false;
            mCloseNodeCounter = 0;
            mOpenNodeValue += 2;
            mCloseNodeValue += 2;

            mOpen.clear();

            Point start = mModel.getXZ(startPos);
            Point end = mModel.getXZ(endPos);

            mLocation = (start.y << mGridXBitWide) + start.x;
            mEndLocation = (end.y << mGridXBitWide) + end.x;

            mCalcGrid[mLocation].G = 0;
            //TODO:
            mCalcGrid[mLocation].F = mHEstimate;

            mCalcGrid[mLocation].PX = (ushort)start.x;
            mCalcGrid[mLocation].PY = (ushort)start.y;
            mCalcGrid[mLocation].Status = mOpenNodeValue;

            mOpen.push(mLocation);

            ushort locationX = 0, locationY = 0, newLocationX = 0, newLocationY = 0;
            int newLocation = 0;
            float newG = 0, heuristic = 0;

            while(mOpen.count > 0 && !mStop)
            {
                mLocation = mOpen.pop();

                PathNodeFast curNode = mCalcGrid[mLocation];
                if (curNode.Status == mCloseNodeValue)
                    continue;

                locationX = (ushort)(mLocation & mGridXMinus);
                locationY = (ushort)(mLocation >> mGridXBitWide);

                if(mLocation == mEndLocation)
                {
                    mCalcGrid[mLocation].Status = mCloseNodeValue;
                    mFound = true;
                    break;
                }

                //search time limit?

                //punish change direction?

                for(int i = 0; i < 8; i++)
                {
                    newLocationX = (ushort)(locationX + mDirection[i, 0]);
                    newLocationY = (ushort)(locationY + mDirection[i, 1]);

                    newLocation = (newLocationY << mGridXBitWide) + newLocationX;

                    if (newLocationX >= mGridX || newLocationY >= mGridY)
                        continue;

                    //Debug.Log("newLocationX: " + newLocationX + " newLocationY: " + newLocationY);
                    if (mModel.getTargetGrid(newLocationX, newLocationY) == NodeState.Block)
                        continue;

                    newG = curNode.G + 1;
                    if (mHeavyDiagonals && i > 3)
                        newG += 0.41f;

                    //if(newLocation >= mCalcGrid.Length)
                    //{
                    //    Debug.Log("newLocation: " + newLocation);
                    //}

                    PathNodeFast newNode = mCalcGrid[newLocation];

                    byte newLocalStatus = newNode.Status;
                    if(newLocalStatus == mOpenNodeValue || newLocalStatus == mCloseNodeValue)
                    {
                        if (newNode.G <= newG)
                            continue;
                    }

                    mCalcGrid[newLocation].PX = locationX;
                    mCalcGrid[newLocation].PY = locationY;
                    mCalcGrid[newLocation].G = newG;


                    heuristic = mHEstimate * (Math.Abs(newLocationX - end.x) + Math.Abs(newLocationY - end.y));
                    mCalcGrid[newLocation].F = newG + heuristic;

                    mOpen.push(newLocation);
                    mCalcGrid[newLocation].Status = mOpenNodeValue;
                }

                mCloseNodeCounter++;
                mCalcGrid[mLocation].Status = mCloseNodeValue;
            }

            if(mFound)
            {
                mClose.Clear();

                int posX = end.x;
                int posY = end.y;

                PathNodeFast fNodeTmp;
                PathNode fNode;

                do
                {
                    fNodeTmp = mCalcGrid[(posY << mGridXBitWide) + posX];
					fNode.F  = fNodeTmp.F;
					fNode.G  = fNodeTmp.G;
					fNode.PX = fNodeTmp.PX;
					fNode.PY = fNodeTmp.PY;
					fNode.X  = posX;
					fNode.Y  = posY;

                    posX = fNode.PX;
                    posY = fNode.PY;

                    if (fNode.X != fNode.PX || fNode.Y != fNode.PY)
                        mClose.Add(fNode);
                    else
                        break;

                }while(true);

                mClose.Add(fNode);

#if UNITY_EDITOR
                mModel.drawPath(mClose);
#endif

                mStopped = true;
                return mClose;
            }

            mStopped = true;
            return null;
        }
    }


}
