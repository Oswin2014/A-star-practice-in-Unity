using UnityEngine;
using System.Collections;


public struct Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator == (Point lhs, Point rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(Point lhs, Point rhs)
    {
        return !(lhs.x == rhs.x && lhs.y == rhs.y);
    }

    public override bool Equals(object obj)
    {
        if (null == obj)
            return false;

        if(obj is Point)
        {
            Point rhs = (Point)obj;
            return x == rhs.x && y == rhs.y;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return x ^ y;
    }

}

public enum HeuristicFormula
{
    Manhattan,
    MaxDXDY,
    DiagonalShortCut,
    Euclidean,
    EuclideanWithG,
    EuclideanNoSQR,
    Custom1
}

public struct PathNode
{
    public float F;
    public float G;
    public int X;
    public int Y;
    public int PX;
    public int PY;
}


public struct PathNodeFast {

    public float F;   // f = gone + heuristic
    public float G;
    public ushort PX;   //parent
    public ushort PY;
    public byte Status;

}


public class NodeState
{
    public const byte Open = 0;
    public const byte Block = 1;
    public const byte DrawPath = 2;
    public const byte DrawCalc = 3;
}