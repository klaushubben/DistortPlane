using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DistortPlane))]
public class DistortPlane_Editor : Editor
{
    protected virtual void OnSceneGUI()
    { 
        DistortPlane plane = (DistortPlane)target;
        Vector3 position = plane.gameObject.transform.localPosition;
        Vector3[] corners = plane.Corners;
 
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < corners.Length; i++)
        { 
            corners[i] = Handles.PositionHandle(position + corners[i], Quaternion.identity);
            corners[i] -= position;
        }

        if (EditorGUI.EndChangeCheck())
        {
            plane.UpdateCorners();
        }
    }

     
}
