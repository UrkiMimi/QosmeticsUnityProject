using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class QosmeticsProjectOverlay
{
    private static bool _isDarkMode;
    private static QosmeticsProjectSettings _settings;

    private static SceneView _currentSceneView;

    static QosmeticsProjectOverlay()
    {
        _isDarkMode = EditorGUIUtility.isProSkin;
        _settings = QosmeticsProjectSettings.GetOrCreateSettings();

        SceneView.duringSceneGui -= DrawGUI;
        SceneView.duringSceneGui += DrawGUI;
    }

    private static void DrawGUI(SceneView sceneView)
    {
        _currentSceneView = sceneView;
        var svEvent = Event.current;

        if (svEvent.type == EventType.Layout)
        {
            if (!_settings.ShowOverlay)
            {
                return;
            }
            var stl = new GUIStyle();

            GUILayout.Window(69420, new Rect(0, 23, sceneView.position.width / 4, 40), DrawWindow, "", stl);
        }
    }

    private static void OpenScene(string scenePath)
    {
        try
        {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene(scenePath);
        }
        catch
        {
            if (EditorUtility.DisplayDialog("Creating New Scene", $"Clicking OK will possibly override the scene located at:\n{scenePath}\nIf this is what you want click ok,\nelse dismiss this dialog box\nANYTHING in that scene will be removed if you click ok anyways and it already existed!", "OK"))
            {
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(newScene, scenePath);
            }

        }
    }
    private static void DrawWindow(int id)
    {
        if (_isDarkMode)
        {
            GUI.color = Color.black;
        }
        GUILayout.BeginHorizontal("box");
        GUI.color = Color.white;
        //GUILayout.Label("Saber Project v" + SaberToolsUpdater.LocalVersionString);
        GUILayout.Label("Qosmetics Project v0.1.0");
        GUILayout.Space(5);
        var blk = new Color(1, 1, 1, _isDarkMode ? 0.4f : 1.0f);
        GUI.color = blk;
        /*
        TODO: updating implementation ?
        if (SaberToolsUpdater.IsUpdateAvailable && !SaberToolsUpdater.IsUpdating)
        {
            if (GUILayout.Button("Update"))
            {
                _ = SaberToolsUpdater.Update();
            }
        }
        */

        if (GUILayout.Button("Settings"))
        {
            QosmeticsProjectSettings.OpenSettingsScreen();
        }
        /*
        if (GUILayout.Button("Exporter"))
        {
            SaberExporterEditor.ShowWindow();
        }
        if (GUILayout.Button("Saber Tools"))
        {
            SaberTools.OpenSaberTools();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Start BeatSaber"))
        {

            if (!_settings.IsSteamPathValid())
            {
                ShowNotification("Please set the steam path");
            }
            else
            {
                StartBeatSaber();
            }
        }
        */

        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Whackers"))
        {
            OpenScene(_settings.WhackerScenePath);
            Qosmetics.Sabers.WhackerTools.OpenSaberTools();
        }
        if (GUILayout.Button("Cyoobs"))
        {
            OpenScene(_settings.CyoobScenePath);
        }
        if (GUILayout.Button("Boxes"))
        {
            OpenScene(_settings.BoxScenePath);
        }

        GUILayout.EndHorizontal();

    }
}
