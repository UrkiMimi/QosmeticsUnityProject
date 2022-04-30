using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

static class QosmeticsProjectSettingsRegister
{

    [SettingsProvider]
    public static SettingsProvider CreateSaberProjectSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider(QosmeticsProjectSettings.SettingsWindowPath, SettingsScope.Project)
        {
            guiHandler = (searchContext) =>
            {
                var settings = QosmeticsProjectSettings.GetSerializedSettings();
                EditorGUIUtility.labelWidth = 200;
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(settings.FindProperty("Author"), new GUIContent("Author Name"));
                GUILayout.Space(10);
                GUILayout.Label("Optional", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(settings.FindProperty("ExportFileName"), new GUIContent("Export Filename"));
                GUILayout.Label("Available templates: {ObjectName}, {ObjectAuthor}, {Extension}");
                GUILayout.Label("Examples: \"{ObjectAuthor}_{ObjectName}.{Extension}\", \"TM_{SaberName}.{Extension}\"");
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
                EditorGUILayout.PropertyField(settings.FindProperty("ShowOverlay"), new GUIContent("Show Overlay"));

                GUILayout.Space(10);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(settings.FindProperty("AllowPushToQuest"), new GUIContent("Allow Pushing To Quest"));
                if (settings.FindProperty("AllowPushToQuest").boolValue)
                {
                    EditorGUILayout.PropertyField(settings.FindProperty("Wireless"), new GUIContent("Wireless"));
                    if (settings.FindProperty("Wireless").boolValue)
                        EditorGUILayout.PropertyField(settings.FindProperty("QuestIPAddress"), new GUIContent("Quest IP Address"));
                    EditorGUILayout.PropertyField(settings.FindProperty("AdbExecutable"), new GUIContent("Path to adb executable"));
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
                GUILayout.Label("Scene paths", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(settings.FindProperty("WhackerScenePath"), new GUIContent("Whacker Scene Path"));
                EditorGUILayout.PropertyField(settings.FindProperty("CyoobScenePath"), new GUIContent("Cyoob Scene Path"));
                EditorGUILayout.PropertyField(settings.FindProperty("BoxScenePath"), new GUIContent("Box Scene Path"));
                EditorGUILayout.EndVertical();

                /*
                // TODO: maybe implement an updater too?
                GUILayout.Space(10);
                GUILayout.Label("Project version " + SaberToolsUpdater.LocalVersion);
                GUILayout.Space(2);
                if (GUILayout.Button("Force Update"))
                {
                    _ = SaberToolsUpdater.Update(true);
                }
                */
                settings.ApplyModifiedProperties();
            },
            keywords = new HashSet<string>(new[] { "Quest IP", "Author Name", "Adb Executable", "Export Filename", "Show Overlay" })
        };

        return provider;
    }
}
