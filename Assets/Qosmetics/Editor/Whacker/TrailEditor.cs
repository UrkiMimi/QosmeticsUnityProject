using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Qosmetics.Sabers;
using Qosmetics.Core;

[CustomEditor(typeof(Trail))]
public class TrailEditor : Editor
{
    bool trailSettingsOpened = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Trail trail = target as Trail;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        trailSettingsOpened = EditorGUILayout.Foldout(trailSettingsOpened, "Trail Settings");
        
        if (trailSettingsOpened)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            ExporterUtils.ObservedObjectField<Transform>(trail.topTransform, "Top", true, newObject => {
                // on changed value
                UnityEditor.Undo.RecordObject(trail, "Change trail top transform");
                trail.topTransform = newObject;
                EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);
            });

            ExporterUtils.ObservedObjectField<Transform>(trail.bottomTransform, "Bottom", true, newObject => {
                // on changed value
                UnityEditor.Undo.RecordObject(trail, "Change trail bottom transform");
                trail.bottomTransform = newObject;
                EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);
            });

            var newColorType = (Trail.ColorType)EditorGUILayout.EnumPopup("ColorType", trail.Colortype);
            if (trail.Colortype != newColorType) {
                UnityEditor.Undo.RecordObject(trail, "Change trail color type");
                trail.Colortype = newColorType;
                EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);
            }
            
            ExporterUtils.ObservedObjectField<Material>(trail.trailMaterial, "Material", false, newObject => {
                UnityEditor.Undo.RecordObject(trail, "Change trail material");
                trail.trailMaterial = newObject;
                EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);

            });

            // only show trail color when relevant
            if (trail.Colortype == Trail.ColorType.Custom) {
                ExporterUtils.ObservedColorField(trail.TrailColor, "Trail Color", newColor => {
                    UnityEditor.Undo.RecordObject(trail, "Change trail color");
                    trail.TrailColor = newColor;
                    EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);
                });
            }

            ExporterUtils.ObservedColorField(trail.MultiplierColor, "Trail Color", newColor => {
                UnityEditor.Undo.RecordObject(trail, "Change trail multiplier color");
                trail.MultiplierColor = newColor;
                EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);
            });

            ExporterUtils.ObservedIntField(trail.Length, "Trail Length", newLength => {
                UnityEditor.Undo.RecordObject(trail, "Change trail length");
                trail.Length = newLength;
                EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);
            });

            ExporterUtils.ObservedSlider(trail.WhiteStep, "Whitestep", 0.0f, 1.0f, newWhiteStep => { 
                UnityEditor.Undo.RecordObject(trail, "Change trail whitestep");
                trail.WhiteStep = newWhiteStep;
                EditorSceneManager.MarkSceneDirty(trail.gameObject.scene);
            });

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawGizmo(Trail trail, GizmoType gizmoType)
    {
        if (!WhackerTools.instance || !WhackerTools.instance.ShowTrailPreview)
        {
            return;
        }


        var mat = trail.trailMaterial;
        if (!mat)
        {
            return;
        }

        var pStartPosition = trail.bottomTransform.localPosition;
        var pEndPosition = trail.topTransform.localPosition;

        var mesh = new Mesh();
        mesh.name = "TrailPreviewMesh";

        var offsetVec = new Vector3(WhackerTools.instance ? trail.Length / 20.0f : 0.5f, 0, 0);

        bool left = false;
        var parent = trail.transform;

        // Traverse the tree upwards and check if there is an object named LeftSaber, if so this is a left trail
        while (parent) {
            if (parent.name == "LeftSaber") {
                left = true;
                break;
            }
            parent = parent.parent;
        }

        if (left && WhackerTools.instance && WhackerTools.instance.FlipLeftTrail) {
            offsetVec.x *= -1.0f;
        }

        mesh.vertices = new[]
        {
            pStartPosition,
            pStartPosition+offsetVec,
            pEndPosition,
            pEndPosition+offsetVec
        };


        mesh.uv = new[]
        {
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };

        if (WhackerTools.instance&& WhackerTools.instance.PreviewCC)
        {
            Color color;
            switch (trail.Colortype)
            {
                case Trail.ColorType.Left:
                    color = WhackerTools.instance.CustomColorLeft;
                    break;
                case Trail.ColorType.Right:
                    color = WhackerTools.instance.CustomColorRight;
                    break;
                case Trail.ColorType.Custom:
                default:
                    color = trail.TrailColor;
                    break;
            }
            color *= trail.MultiplierColor;
            mesh.colors = new[] { color, color, color, color };
        }

        int[] triangles = new int[6];
        
        for (int ti = 0, vi = 0, y = 0; y < 1; y++, vi++)
        {
            for (int x = 0; x < 1; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + 1 + 1;
                triangles[ti + 5] = vi + 1 + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mat.SetPass(0);
        Graphics.DrawMeshNow(mesh, trail.bottomTransform.parent.localToWorldMatrix);
    }
}
