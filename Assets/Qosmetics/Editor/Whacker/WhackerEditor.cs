using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Qosmetics.Sabers.Whacker))]
public class WhackerEditor : Editor
{
    public static string Extension { get => "whacker"; }
    bool packageSettingsOpened = true;
    bool objectSettingsOpened = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Qosmetics.Sabers.Whacker whacker = target as Qosmetics.Sabers.Whacker;

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        packageSettingsOpened = EditorGUILayout.Foldout(packageSettingsOpened, "Package Settings");
        if (packageSettingsOpened)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            whacker.ObjectName = EditorGUILayout.TextField("Name", whacker.ObjectName);
            whacker.Author = EditorGUILayout.TextField("Author", whacker.Author);
            whacker.Description = EditorGUILayout.TextField("Description", whacker.Description);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        /*
        // There are no object settings for whackers atm
        objectSettingsOpened = EditorGUILayout.Foldout(objectSettingsOpened, "Object Settings");
        if (objectSettingsOpened)
        {
            whacker.ShowArrows = EditorGUILayout.ToggleLeft("Use Base Game Arrows", whacker.ShowArrows);
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        */

        if (whacker.ValidateObject())
        {
            if (GUILayout.Button($"Export {whacker.GetType().Name}"))
            {
                string path = EditorUtility.SaveFilePanel($"Save {Extension} file", "", $"{whacker.ObjectName}.{Extension}", Extension);
                if (path != "") Qosmetics.Core.ExporterUtils.ExportAsPrefabPackage(whacker.gameObject, $"_{whacker.GetType().Name}", path);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Your whacker is improperly made!", GUI.skin.button);
            EditorGUILayout.LabelField("Check if there is a LeftSaber and RightSaber");
            EditorGUILayout.LabelField("Other than that check if all trails have a material");

        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.EndVertical();
    }

}
