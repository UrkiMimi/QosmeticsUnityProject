using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Qosmetics.Core
{
    public class Theme : ScriptableObject
    {
        //private const string ThemeBasePath = "Packages/com.qosmeticscore.redbrumbler.qosmeticstoolkit/PersistentData/";
        private const string ThemeBasePath = "Assets/Qosmetics/PersistentData";

        [SerializeField] public string Name;

        [SerializeField] public Color BackgroundColor = Color.white;
        [SerializeField] public Color HeaderColor = Color.black;
        [SerializeField] public Color WarningColor = Color.yellow;
        [SerializeField] public Color ErrorColor = Color.red;
        [SerializeField] public Color SuccessColor = Color.green;

        private static Theme _instance;

        public static Theme GetTheme()
        {
            if (!_instance || !IsCorrectSkin(_instance))
            {
                var filename = ThemeBasePath + "/" + (EditorGUIUtility.isProSkin ? "DarkTheme.asset" : "LightTheme.asset");
                var theme = AssetDatabase.LoadAssetAtPath<Theme>(filename);
                if (!theme)
                {
                    Debug.LogWarning($"Failed to load theme: {filename}");
                    theme = CreateInstance<Theme>();
                    try
                    {
                        AssetDatabase.CreateAsset(theme, filename);
                        AssetDatabase.SaveAssets();
                    }
                    catch (Exception _){ }
                }
                _instance = theme;
            }

            return _instance;
        }

        public static bool IsCorrectSkin(Theme theme)
        {
            var isDark = EditorGUIUtility.isProSkin;
            return (theme.Name == "Light" && !isDark) || (theme.Name == "Dark" && isDark);
        }
    }
}