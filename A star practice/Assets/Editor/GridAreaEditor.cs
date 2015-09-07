using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GridArea))]
public class GridAreaEditor : Editor
{

    GridArea m_area;

    void OnEnable()
    {
        m_area = (GridArea)this.target;
    }

    void OnSceneGUI()
    {

    }


}
