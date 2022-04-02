using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Qosmetics.Sabers.Trail))]
public class TrailEditor : Editor
{
    bool trailSettingsOpened = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Qosmetics.Sabers.Trail trail= target as Qosmetics.Sabers.Trail;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        trailSettingsOpened = EditorGUILayout.Foldout(trailSettingsOpened, "Trail Settings");
        if (trailSettingsOpened)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            trail.topTransform = EditorGUILayout.ObjectField("Top", trail.topTransform, typeof(Transform), true) as Transform;
            trail.bottomTransform = EditorGUILayout.ObjectField("Bottom", trail.bottomTransform, typeof(Transform), true) as Transform;
            trail.Colortype = (Qosmetics.Sabers.Trail.ColorType)EditorGUILayout.EnumPopup("ColorType", trail.Colortype);
            if (trail.Colortype == Qosmetics.Sabers.Trail.ColorType.Custom)
            trail.TrailColor = EditorGUILayout.ColorField("Trail Color", trail.TrailColor);
            trail.MultiplierColor = EditorGUILayout.ColorField("Multiplier Color", trail.MultiplierColor);
            trail.Length = EditorGUILayout.IntField("Trail Length", trail.Length);
            trail.WhiteStep = EditorGUILayout.Slider("Whitestep", trail.WhiteStep, 0.0f, 1.0f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
