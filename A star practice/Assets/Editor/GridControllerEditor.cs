using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GridController))]
public class GridControllerEditor : Editor
{

    GridController mController;

    int mState;

    void OnEnable()
    {
        mController = (GridController)this.target;
    }

    void OnSceneGUI()
    {
        if (0 == mState)
            return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        int id = GUIUtility.GetControlID(FocusType.Passive);
        EventType type = Event.current.GetTypeForControl(id);

        if ((type == EventType.MouseDown || type == EventType.MouseDrag) && Event.current.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            bool ok = Physics.Raycast(ray, out hit, 1000, GenericHelper.GetLayerMask("Ignore Raycast"));
            if (ok)
            {
                mController.setBlock(hit.point, mState % 2);
            }
        }

        HandleUtility.Repaint();
        Repaint();
    }

    public override void OnInspectorGUI()
    {

        string[] state = { "off", "full", "free" };
        mState = GUILayout.Toolbar(mState, state);
    }

}
