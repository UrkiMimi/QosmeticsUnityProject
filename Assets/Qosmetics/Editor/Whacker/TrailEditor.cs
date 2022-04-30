using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Qosmetics.Sabers;

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
            trail.topTransform = EditorGUILayout.ObjectField("Top", trail.topTransform, typeof(Transform), true) as Transform;
            trail.bottomTransform = EditorGUILayout.ObjectField("Bottom", trail.bottomTransform, typeof(Transform), true) as Transform;
            trail.Colortype = (Trail.ColorType)EditorGUILayout.EnumPopup("ColorType", trail.Colortype);
            trail.trailMaterial = EditorGUILayout.ObjectField("Material", trail.trailMaterial, typeof(Material), false) as Material;
            if (trail.Colortype == Trail.ColorType.Custom)
                trail.TrailColor = EditorGUILayout.ColorField("Trail Color", trail.TrailColor);
            trail.MultiplierColor = EditorGUILayout.ColorField("Multiplier Color", trail.MultiplierColor);
            trail.Length = EditorGUILayout.IntField("Trail Length", trail.Length);
            trail.WhiteStep = EditorGUILayout.Slider("Whitestep", trail.WhiteStep, 0.0f, 1.0f);
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
