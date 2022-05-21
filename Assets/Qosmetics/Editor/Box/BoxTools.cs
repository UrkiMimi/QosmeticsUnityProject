using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Qosmetics.Core;

namespace Qosmetics.Walls
{
    public class BoxTools : EditorWindow
    {
        static public BoxTools instance;
        private Vector2 _scrollPos = Vector2.zero;
        Box selectedBox = null;

        QosmeticsProjectSettings _projectSettings;
        bool _isCreateWallOpen = false;
        bool _isOtherToolsOpen = false;

        string _templateText = "";

        Mesh _templateCoreMesh;
        Mesh CoreMesh
        {
            get
            {
                return _templateCoreMesh ? _templateCoreMesh : AssetDatabase.LoadAssetAtPath("Assets/Qosmetics/Models/WallCore.fbx", typeof(Mesh)) as Mesh;
            }
        }
        Mesh _templateFrameMesh;
        Mesh FrameMesh
        {
            get
            {
                return _templateFrameMesh ? _templateFrameMesh : AssetDatabase.LoadAssetAtPath("Assets/Qosmetics/Models/WallHoleAllSides.fbx", typeof(Mesh)) as Mesh;
            }
        }
        Material _templateCoreMaterial;
        Material CoreMaterial
        {
            get
            {
                return _templateCoreMaterial ? _templateCoreMaterial : AssetDatabase.LoadAssetAtPath("Assets/Qosmetics/Materials/WallCore.mat", typeof(Material)) as Material;
            }
        }
        Material _templateFrameMaterial;
        Material FrameMaterial
        {
            get
            {
                return _templateFrameMaterial ? _templateFrameMaterial : AssetDatabase.LoadAssetAtPath("Assets/Qosmetics/Materials/WallFrame.mat", typeof(Material)) as Material;
            }
        }

        [MenuItem("Qosmetics/Box Tools")]
        public static void OpenWallTools()
        {
            instance = GetWindow<BoxTools>(false, "Box Tools");
        }

        Theme _theme;

        public void OnFocus()
        {
            _projectSettings = QosmeticsProjectSettings.GetOrCreateSettings();
            _theme = Theme.GetTheme();
        }

        public void GetSelectedBox()
        {
            if (Selection.activeGameObject)
            {
                selectedBox = Selection.activeGameObject.GetComponent<Box>();
            }
            else
            {
                selectedBox = null;
            }
        }

        public void OnGUI()
        {
            GetSelectedBox();

            GUILayout.Space(10);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Create Box", _theme.HeaderColor);
            UITools.Foldout(ref _isCreateWallOpen);
            if (_isCreateWallOpen)
            {
                UITools.Header("General", Color.cyan);
                _templateText = EditorGUILayout.TextField("Name", _templateText);
                GUILayout.Space(2);

                _templateCoreMesh =
                    (Mesh)EditorGUILayout.ObjectField("Core Mesh", _templateCoreMesh, typeof(Mesh), false);
                _templateFrameMesh =
                    (Mesh)EditorGUILayout.ObjectField("Frame Mesh", _templateFrameMesh, typeof(Mesh), false);
                _templateCoreMaterial =
                    (Material)EditorGUILayout.ObjectField("Core Material", _templateCoreMaterial, typeof(Material), false);
                _templateFrameMaterial =
                    (Material)EditorGUILayout.ObjectField("Frame Material", _templateFrameMaterial, typeof(Material), false);
                GUILayout.Space(10);
                if (GUILayout.Button("Create Template", GUILayout.Height(20)))
                {
                    CreateTemplate();
                }
            }
            UITools.EndSection();

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Other Tools", _theme.HeaderColor);
            UITools.Foldout(ref _isOtherToolsOpen);
            if (_isOtherToolsOpen)
            {
                if (UITools.Button("Select All Renderers"))
                {
                    Selection.objects = ExporterUtils.SelectAllRenderers(Selection.activeGameObject).ToArray();
                }

                if (UITools.Button("Detect Proper Frame"))
                {
                    DetectFrameMaterial();
                }

                if (UITools.Button("Detect Mirrorable"))
                {
                    ExporterUtils.DetectMirrorable(selectedBox?.gameObject);
                }

            }
            UITools.EndSection();


            GUILayout.EndScrollView();
        }

        public void CreateTemplate()
        {
            var rootGo = new GameObject(_templateText);
            var wallDescriptor = rootGo.AddComponent<Box>();
            wallDescriptor.ObjectName = _templateText;

            if (_projectSettings)
            {
                wallDescriptor.Author = _projectSettings.Author;
            }

            var coreGo = new GameObject("Core");
            coreGo.transform.parent = rootGo.transform;
            coreGo.transform.localScale = Vector3.one * .99f;

            var frameGo = new GameObject("Frame");
            frameGo.transform.parent = rootGo.transform;
            frameGo.transform.localScale = Vector3.one * .5f;

            var coreFilter = coreGo.AddComponent<MeshFilter>();
            var coreRenderer = coreGo.AddComponent<MeshRenderer>();
            var frameFilter = frameGo.AddComponent<MeshFilter>();
            var frameRenderer = frameGo.AddComponent<MeshRenderer>();

            coreFilter.sharedMesh = CoreMesh;
            frameFilter.sharedMesh = FrameMesh;
            coreRenderer.sharedMaterial = CoreMaterial;
            frameRenderer.sharedMaterial = FrameMaterial;

            var sizeParams = rootGo.AddComponent<sizeParamsScript>();
            sizeParams.frameSizeParams = true;

            rootGo.transform.localScale = new Vector3(2, 3, 8);
        }


        public void DetectFrameMaterial()
        {
            if (selectedBox)
            {
                var frame = selectedBox.transform.Find("Frame")?.gameObject;
                if (!frame)
                {
                    EditorUtility.DisplayDialog("No Frame found", "This box does not have a frame", "OK");
                    return;
                }

                var renderer = frame.GetComponent<MeshRenderer>();
                if (!renderer)
                {
                    EditorUtility.DisplayDialog("No renderer found", "The frame object does not have a meshrenderer", "OK");
                    return;
                }

                foreach (var mat in renderer.sharedMaterials)
                {
                    if (!mat.HasProperty("_SizeParams"))
                    {
                        EditorUtility.DisplayDialog("Bad frame material", "The frame material does not have the neccesary property to function the same way the normal frames do", "OK");
                        return;
                    }
                }

                EditorUtility.DisplayDialog("Proper frame material", "The frame material has the properties to work like the in game frames!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Nothing selected", "You do not have anything selected that has a Box component on the object or it's parents, can't verify proper frame material.", "OK");
            }
        }

    }
}