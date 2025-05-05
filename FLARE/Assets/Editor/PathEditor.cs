using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    void OnSceneGUI()
    {
        Path path = (Path)target;

        for (int i = 0; i < path.waypoints.Count; i++)
        {
            if (path.DrawNumbers)
            {
                GUIStyle labelStyle = new GUIStyle();
                labelStyle.fontSize = 30;
                labelStyle.normal.textColor = path.debugColour;
                Handles.Label(path.waypoints[i].position, i.ToString(), labelStyle);
            }
        }
    }
}
