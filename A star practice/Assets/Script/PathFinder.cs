
//#define Draw_Calc_Node

//#define Diagnostic

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

    ushort mGridX;
    ushort mGridY;

    ushort mGridXBitWide;
    ushort mGridXMinus;

    byte mOpenNodeValue = 1;
    byte mCloseNodeValue = 2;

    public HeuristicFormula formula = HeuristicFormula.Manhattan;

    public bool tieBreaker = false;
    public bool punishChangeDirection = false;

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

    public List<PathNode> FindPath(Vector3 startPos, Vector3 endPos
#if UNITY_EDITOR && Draw_Calc_Node
        , ref Dictionary<int, PathNode> calcNodeDic
#endif
)
    {
        lock(this)
        {

#if Diagnostic
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif

#if UNITY_EDITOR && Draw_Calc_Node
            calcNodeDic.Clear();
            PathNode calcNode;
            calcNode.F = 0;
            calcNode.G = 0;
            calcNode.PX = 0;
            calcNode.PY = 0;
#endif

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
            mCalcGrid[mLocation].F = Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y);
            mCalcGrid[mLocation].PX = (ushort)start.x;
            mCalcGrid[mLocation].PY = (ushort)start.y;
            mCalcGrid[mLocation].Status = mOpenNodeValue;

            mOpen.push(mLocation);

            ushort locationX = 0, locationY = 0, newLocationX = 0, newLocationY = 0;
            int newLocation = 0, horiz = 0;
            float newG = 0, heuristic = 0;


#if Diagnostic
            sw.Stop();
            Debug.Log(sw.ElapsedTicks / 10000.0f);

            sw.Reset();
            sw.Start();
#endif

            while (mOpen.count > 0 && !mStop)
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
                if (punishChangeDirection)
                    horiz = locationX - mCalcGrid[mLocation].PX;

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

                    if(punishChangeDirection)
                    {
                        if( (newLocationX - locationX) != 0 )
                        {
                            if(0 == horiz)
                                newG += Math.Abs(newLocationX - end.x) + Math.Abs(newLocationY - end.y);
                        }
                        if( 0 != (newLocationY - locationY) )
                        {
                            if(0 != horiz)
                                newG += Math.Abs(newLocationX - end.x) + Math.Abs(newLocationY - end.y);
                        }
                    }

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

                    
                    switch(formula)
                    {
                        case HeuristicFormula.Manhattan:
                            heuristic = Math.Abs(newLocationX - end.x) + Math.Abs(newLocationY - end.y);
                            break;
                        case HeuristicFormula.MaxDXDY:
                            heuristic = Math.Max(Math.Abs(newLocationX - end.x), Math.Abs(newLocationY - end.y));
                            break;
                        case HeuristicFormula.DiagonalShortCut:
                            {
                                int xDist = Math.Abs(newLocationX - end.x);
                                int yDist = Math.Abs(newLocationY - end.y);
                                if(xDist > yDist)
                                    heuristic = 1.4f * yDist + (xDist - yDist);
                                else
                                    heuristic = 1.4f * xDist + (yDist - xDist);
                            }
                            break;
                        case HeuristicFormula.Euclidean:
                            heuristic = Mathf.Sqrt( Mathf.Pow(Math.Abs(newLocationX - end.x), 2) + Mathf.Pow(Math.Abs(newLocationY - end.y), 2) );
                            break;
                        case HeuristicFormula.EuclideanWithG:
                            heuristic = Mathf.Sqrt(Mathf.Pow(Math.Abs(newLocationX - end.x), 2) + Mathf.Pow(Math.Abs(newLocationY - end.y), 2));
                            newG *= newG;
                            break;
                        case HeuristicFormula.EuclideanNoSQR:
                            heuristic = Mathf.Pow(Math.Abs(newLocationX - end.x), 2) + Mathf.Pow(Math.Abs(newLocationY - end.y), 2);
                            break;
                        case HeuristicFormula.Custom1:
                            {
                                int xDist = Math.Abs(newLocationX - end.x);
                                int yDist = Math.Abs(newLocationY - end.y);
                                int Orthogonal = Math.Abs(xDist - yDist);
                                int Diagonal = Math.Abs( ( (xDist + yDist) - Orthogonal ) / 2 );
                                heuristic = Diagonal + Orthogonal + xDist + yDist;
                            }
                            break;
                    }

                    if(tieBreaker)
                    {
                        int dx1 = locationX - end.x;
                        int dy1 = locationY - end.y;
                        int dx2 = start.x - end.x;
                        int dy2 = start.y - end.y;
                        int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
                        heuristic += cross * 0.001f;
                    }

                    mCalcGrid[newLocation].F = newG + heuristic;

#if UNITY_EDITOR && Draw_Calc_Node
                    if(!calcNodeDic.ContainsKey(newLocation))
                    {
                        calcNode.X = newLocationX;
                        calcNode.Y = newLocationY;
                        calcNodeDic[newLocation] = calcNode;
                    }
#endif

                    mOpen.push(newLocation);
                    mCalcGrid[newLocation].Status = mOpenNodeValue;
                }

                mCloseNodeCounter++;
                mCalcGrid[mLocation].Status = mCloseNodeValue;
            }
            
#if Diagnostic
            sw.Stop();
            Debug.Log(sw.ElapsedTicks / 10000.0f);
#endif

            if(mFound)
            {
#if Diagnostic
                sw.Reset();
                sw.Start();
#endif

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
                
#if Diagnostic
                sw.Stop();
                Debug.Log(sw.ElapsedTicks / 10000.0f);
#endif

                mStopped = true;
                return mClose;
            }

            mStopped = true;
            return null;
        }
    }


}
