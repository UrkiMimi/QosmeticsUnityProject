using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Qosmetics.Notes.Cyoob))]
public class CyoobEditor : Editor
{
    public static string Extension { get => "cyoob"; }
    bool packageSettingsOpened = true;
    bool objectSettingsOpened = true;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Qosmetics.Notes.Cyoob cyoob = target as Qosmetics.Notes.Cyoob;

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        packageSettingsOpened = EditorGUILayout.Foldout(packageSettingsOpened, "Package Settings");
        if (packageSettingsOpened)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            cyoob.ObjectName = EditorGUILayout.TextField("Name", cyoob.ObjectName);
            cyoob.Author = EditorGUILayout.TextField("Author", cyoob.Author);
            cyoob.Description = EditorGUILayout.TextField("Description", cyoob.Description);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        objectSettingsOpened = EditorGUILayout.Foldout(objectSettingsOpened, "Object Settings");
        if (objectSettingsOpened)
        {
            cyoob.ShowArrows = EditorGUILayout.ToggleLeft("Use Base Game Arrows", cyoob.ShowArrows);
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (cyoob.ValidateObject())
        {
            if (GUILayout.Button($"Export {cyoob.GetType().Name}"))
            {
                string path = EditorUtility.SaveFilePanel($"Save {Extension} file", "", $"{cyoob.ObjectName}.{Extension}" , Extension);
                if (path != "") Qosmetics.Core.ExporterUtils.ExportAsPrefabPackage(cyoob.gameObject, $"_{cyoob.GetType().Name}", path);
            }
        }
        else
        {
            EditorGUILayout.LabelField("You're missing the arrow & dot Cyoobs!", GUI.skin.button);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.EndVertical();
    }
}
