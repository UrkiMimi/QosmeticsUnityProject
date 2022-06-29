using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Qosmetics.Core;
using System.IO;

[CustomEditor(typeof(Qosmetics.Sabers.Whacker))]
public class WhackerEditor : Editor
{
    public static string Extension { get => "whacker"; }
    bool packageSettingsOpened = true;
    bool thumbnailSettingsOpened = true;
    QosmeticsProjectSettings _projectSettings = null;
    public override void OnInspectorGUI()
    {
        if (!_projectSettings)
        {
            _projectSettings = QosmeticsProjectSettings.GetOrCreateSettings();
        }

        serializedObject.Update();
        Qosmetics.Sabers.Whacker whacker = target as Qosmetics.Sabers.Whacker;

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        packageSettingsOpened = EditorGUILayout.Foldout(packageSettingsOpened, "Package Settings");
        if (packageSettingsOpened)
        {
            EditorGUILayout.BeginVertical("box");
            whacker.ObjectName = EditorGUILayout.TextField("Name", whacker.ObjectName);
            whacker.Author = EditorGUILayout.TextField("Author", whacker.Author);
            whacker.Description = EditorGUILayout.TextField("Description", whacker.Description);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        thumbnailSettingsOpened = EditorGUILayout.Foldout(thumbnailSettingsOpened, "Thumbnail Selection");
        if (thumbnailSettingsOpened)
        {
            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button("Generate Thumbnail"))
            {
                var path = ExporterUtils.GenerateThumbnail();
                if (string.IsNullOrEmpty(path))
                {
                    EditorUtility.DisplayDialog("Thumbnail Generation failed", "Failed to properly generate thumbnail", "OK");
                }
                else
                    whacker.Thumbnail = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
            }

            whacker.Thumbnail = EditorGUILayout.ObjectField("Thumbnail", whacker.Thumbnail, typeof(Texture2D), false) as Texture2D;
            EditorGUILayout.EndVertical();
        }
        /*
        // There are no object settings for whackers atm
        objectSettingsOpened = EditorGUILayout.Foldout(objectSettingsOpened, "Object Settings");
        if (objectSettingsOpened)
        {
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        */
        string validationString = whacker.ValidateObject();
        if (string.IsNullOrWhiteSpace(validationString))
        {
            if (GUILayout.Button($"Export {whacker.GetType().Name}"))
            {
                Export(whacker);
            }

            if (_projectSettings.AllowPushToQuest && _projectSettings.IsAdbValid() && GUILayout.Button($"Push {whacker.GetType().Name} To Quest"))
            {
                string path = Export(whacker);
                if (!string.IsNullOrEmpty(path))
                    Qosmetics.Core.AdbUtils.Push(path, $"/sdcard/ModData/com.beatgames.beatsaber/Mods/Qosmetics/Boxes/{Path.GetFileName(path)}");
            }
        }
        else
        {
            EditorGUILayout.LabelField("Your whacker is improperly made!", GUI.skin.button);
            EditorGUILayout.LabelField(validationString, GUI.skin.button);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.EndVertical();
    }

    string Export(Qosmetics.Sabers.Whacker whacker)
    {
        string exportName = _projectSettings.ExportFileName;
        exportName = exportName.Replace("{ObjectName}", whacker.ObjectName);
        exportName = exportName.Replace("{ObjectAuthor}", whacker.Author);
        exportName = exportName.Replace("{Extension}", Extension);

        string path = EditorUtility.SaveFilePanel($"Save {Extension} file", "", exportName, Extension);
        if (!string.IsNullOrEmpty(path)) Qosmetics.Core.ExporterUtils.ExportAsPrefabPackage(whacker.gameObject, $"_{whacker.GetType().Name}", path, whacker.Thumbnail);
        return path;
    }

}
