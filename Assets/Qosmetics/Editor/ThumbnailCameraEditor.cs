using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Qosmetics.Core.ThumbnailCamera))]
public class ThumbnailCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Qosmetics.Core.ThumbnailCamera cam = target as Qosmetics.Core.ThumbnailCamera;

        GUILayout.BeginVertical();
        cam.width = EditorGUILayout.IntField("Texture Width", cam.width);
        cam.height = EditorGUILayout.IntField("Texture Height", cam.height);
        GUILayout.EndVertical();
        GUILayout.Space(2);
        if (GUILayout.Button("Align with view"))
        {
            cam.transform.position = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
            cam.transform.rotation = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
        }
    }

}
