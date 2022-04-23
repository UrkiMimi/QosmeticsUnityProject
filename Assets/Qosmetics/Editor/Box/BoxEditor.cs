using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Qosmetics.Walls.Box))]
public class BoxEditor : Editor
{
    public static string Extension { get => "box"; }
    bool packageSettingsOpened = true;
    bool objectSettingsOpened = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Qosmetics.Walls.Box box = target as Qosmetics.Walls.Box;

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        packageSettingsOpened = EditorGUILayout.Foldout(packageSettingsOpened, "Package Settings");
        if (packageSettingsOpened)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            box.ObjectName = EditorGUILayout.TextField("Name", box.ObjectName);
            box.Author = EditorGUILayout.TextField("Author", box.Author);
            box.Description = EditorGUILayout.TextField("Description", box.Description);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // There are no object settings for whackers atm
        objectSettingsOpened = EditorGUILayout.Foldout(objectSettingsOpened, "Object Settings");
        if (objectSettingsOpened)
        {
            box.replaceCoreMaterial = EditorGUILayout.ToggleLeft("Replace Core Material", box.replaceCoreMaterial);
            box.replaceFrameMaterial = EditorGUILayout.ToggleLeft("Replace Frame Material", box.replaceFrameMaterial);
            box.replaceCoreMesh = EditorGUILayout.ToggleLeft("Replace Core Mesh", box.replaceCoreMesh);
            box.replaceFrameMesh = EditorGUILayout.ToggleLeft("Replace Frame Mesh", box.replaceFrameMesh);
            box.disableCore = EditorGUILayout.ToggleLeft("Disable Core", box.disableCore);
            box.disableFrame = EditorGUILayout.ToggleLeft("Disable Frame", box.disableFrame);
            box.disableFakeGlow = EditorGUILayout.ToggleLeft("Disable Fake Glow", box.disableFakeGlow);
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        string validationString = box.ValidateObject();

        if (string.IsNullOrEmpty(validationString))
        {
            if (GUILayout.Button($"Export {box.GetType().Name}"))
            {
                string path = EditorUtility.SaveFilePanel($"Save {Extension} file", "", $"{box.ObjectName}.{Extension}", Extension);
                if (path != "") Qosmetics.Core.ExporterUtils.ExportAsPrefabPackage(box.gameObject, $"_{box.GetType().Name}", path);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Your box is improperly made!", GUI.skin.button);
            EditorGUILayout.LabelField(validationString, GUI.skin.button);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.EndVertical();
    }
    
}
