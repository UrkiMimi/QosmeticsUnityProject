using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class QosmeticsProjectSettings : ScriptableObject
{
    public const string SettingsPath = "Assets/Qosmetics/ProjectSettings.asset";
    public const string SettingsWindowPath = "Project/Qosmetics Exporter";

    [SerializeField] public string Author;
    [SerializeField] public string ExportFileName = "{ObjectName}.{Extension}";
    [SerializeField] public bool ShowOverlay = true;
    [SerializeField] public bool AllowPushToQuest = false;
    [SerializeField] public bool Wireless = false;
    [SerializeField] public string QuestIPAddress = "192.168.0.20";
    [SerializeField] public string AdbExecutable = "adb.exe";

    [SerializeField] public string WhackerScenePath = "Assets/Scenes/Whackers.unity";
    [SerializeField] public string CyoobScenePath = "Assets/Scenes/Cyoobs.unity";
    [SerializeField] public string BoxScenePath = "Assets/Scenes/Boxes.unity";

    bool adbValid = false;
    internal bool IsAdbValid()
    {
        if (adbValid) return true;
        return (adbValid = (Process.Start(AdbExecutable, "--version") != null));
    }

    internal static QosmeticsProjectSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<QosmeticsProjectSettings>(SettingsPath);
        if (settings == null)
        {
            string dirPath = Path.Combine(Application.dataPath, SettingsPath.Substring(7));
            if (Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            
            settings = CreateInstance<QosmeticsProjectSettings>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    internal static void OpenSettingsScreen()
    {
        SettingsService.OpenProjectSettings(SettingsWindowPath);
    }
}
