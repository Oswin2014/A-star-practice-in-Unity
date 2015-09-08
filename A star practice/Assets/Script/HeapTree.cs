
using System.Collections;
using System.Collections.Generic;

public class HeapTree {

    protected List<int> innerList = new List<int>();
    protected PathNodeFast[] mMatrix;

    public int count
    {
        get { return innerList.Count; }
    }

    public HeapTree(PathNodeFast[] matrix)
    {
        mMatrix = matrix;
    }

    public void clear()
    {
        innerList.Clear();
    }
    
    protected virtual float compare(int i, int j)
    {
        return compareWithIndex(innerList[i], innerList[j]);
    }

    public virtual float compareWithIndex(int i, int j)
    {
        return mMatrix[i].F - mMatrix[j].F;
    }

    public int push(int item)
    {
        int p = innerList.Count, pRoot;
        innerList.Add(item);

        do
        {
            if (0 == p)
                break;

            //compare with parent node
            pRoot = (p - 1) / 2;
            if (compare(p, pRoot) < 0)
            {
                swap(p, pRoot);
                p = pRoot;
            }
            else
                break;

        } while (true);

        return p;
    }

    public int pop()
    {
        if (innerList.Count < 1)
            return -1;

        int ret = innerList[0];

        int count = innerList.Count;
        int temp = innerList[count - 1];
        innerList.RemoveAt(count - 1);
        count--;

        if (count < 1)
            return ret;

        //adjust heap tree
        int l = 0;
        for (int i = 1 /*(l * 2 + 1)*/; i < count; i = i * 2 + 1)
        {
            if (i + 1 < count && compare(i, i + 1) > 0)
                i++;

            if (compareWithIndex(temp, innerList[i]) <= 0)
                break;

            innerList[l] = innerList[i];
            l = i;
        }
        innerList[l] = temp;

        return ret;
    }

    void swap(int i, int j)
    {
        int temp = innerList[i];
        innerList[i] = innerList[j];
        innerList[j] = temp;
    }




}
