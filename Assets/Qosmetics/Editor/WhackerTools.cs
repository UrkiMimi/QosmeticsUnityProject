using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Qosmetics.Core;

namespace Qosmetics.Sabers
{
    public class WhackerTools : EditorWindow
    {
        static public WhackerTools instance;
        public const float SaberLength = 1.179f;
        private const float SaberOffset = 0.1745f;

        private Vector2 _scrollPos = Vector2.zero;
        
        // Debug visuals
        public bool ShowWhackerGuides = true;
        public bool ShowTrailGuides = true;
        public bool ShowTrailPreview = false;
        
        // Preview visuals
        public bool PreviewCC = true;
        public bool BeatSaberLookActive;

        public Color CustomColorLeft = Color.red;
        public Color CustomColorRight = Color.cyan;

        QosmeticsProjectSettings _projectSettings;
        Whacker selectedWhacker = null;

        bool _isGuidesOpen = false;
        bool _isCreateSaberOpen = false;
        bool _isFixingOpen = false;
        bool _isBeatSaberLookOpen = false;
        bool _isOtherToolsOpen = false;

        string _templateText = "";
        bool _createRightSaber = false;
        GameObject _templatePrefab = null;
        Material _trailMaterial = null;
        int _trailLength = 14;
        float _spacingBetweenSabers = 0.3f;
        float _trailWidth = SaberLength;
        

        [MenuItem("Qosmetics/Whacker Tools")]
        public static void OpenSaberTools()
        {
            instance = GetWindow<WhackerTools>(false, "Whacker Tools");
        }

        Theme _theme;

        public void OnFocus()
        {
            _projectSettings = QosmeticsProjectSettings.GetOrCreateSettings();

            if (Selection.activeGameObject)
            {
                selectedWhacker = Selection.activeGameObject.GetComponent<Whacker>();
            }
            else
            {
                selectedWhacker = null;
            }

            _theme = Theme.GetTheme();
        }

        public void OnGUI()
        {
            GUILayout.Space(10);
            
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Visualizers", _theme.HeaderColor);
            UITools.Foldout(ref _isGuidesOpen);
            if (_isGuidesOpen)
            {
                UITools.ChangedToggle(ref ShowWhackerGuides, "Show Guides", val =>
                {
                    SceneView.RepaintAll();
                });

                UITools.ChangedToggle(ref ShowTrailGuides, "Show Trail Guides", val =>
                {
                    SceneView.RepaintAll();
                });

                UITools.ChangedToggle(ref ShowTrailPreview, "Show Trail Preview", val =>
                {
                    SceneView.RepaintAll();
                });

                if (ShowTrailPreview)
                {
                    UITools.ChangedToggle(ref PreviewCC, "- Preview vertex CC", val =>
                    {
                        SceneView.RepaintAll();
                    });
                }
            }
            UITools.EndSection();

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Create Whacker", _theme.HeaderColor);
            UITools.Foldout(ref _isCreateSaberOpen);
            if (_isCreateSaberOpen)
            {
                UITools.Header("General", Color.cyan);
                _templateText = EditorGUILayout.TextField("Name", _templateText);
                GUILayout.Space(2);
                _createRightSaber = EditorGUILayout.Toggle("Create RightSaber", _createRightSaber);
                if (_createRightSaber)
                {
                    _spacingBetweenSabers = EditorGUILayout.FloatField("Spacing between sabers", _spacingBetweenSabers);
                }
                _templatePrefab =
                    (GameObject)EditorGUILayout.ObjectField("Template Prefab", _templatePrefab, typeof(GameObject), false);
                GUILayout.Space(5);
                UITools.Header("Trails", Color.cyan);
                _trailMaterial = (Material)EditorGUILayout.ObjectField("Trail Material", _trailMaterial, typeof(Material), false);
                _trailLength = EditorGUILayout.IntField("Trail Length", _trailLength);
                _trailWidth = EditorGUILayout.Slider("Trail Width", _trailWidth, 0, SaberLength);
                GUILayout.Space(10);
                if (GUILayout.Button("Create Template", GUILayout.Height(20)))
                {
                    CreateTemplate();
                }
            }
            UITools.EndSection();

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Fixing", _theme.HeaderColor);
            UITools.Foldout(ref _isFixingOpen);
            if (_isFixingOpen)
            {
                if (UITools.Button("Fix Length"))
                {
                    FixLength();
                }
            }
            UITools.EndSection();

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Beat Saber Look", _theme.HeaderColor);
            UITools.Foldout(ref _isBeatSaberLookOpen);
            if (_isBeatSaberLookOpen)
            {
                GUILayout.BeginHorizontal();
                UITools.ChangedColor(ref CustomColorLeft, "Left Color", col => { SceneView.RepaintAll(); });
                UITools.ChangedColor(ref CustomColorRight, "Right Color", col => { SceneView.RepaintAll(); });
                GUILayout.EndHorizontal();
                
                if (UITools.Button("Apply"))
                {
                    if (Selection.activeGameObject)
                    {
                        var whacker = Selection.activeGameObject.GetComponent<Whacker>();
                        if (!whacker)
                        {
                            whacker = Selection.activeGameObject.GetComponentInParent<Whacker>();
                        }

                        if (whacker) ColorWhacker(whacker);
                    }
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
                    Selection.objects = SelectAllRenderers(Selection.activeGameObject).ToArray();
                }

                GUILayout.Space(5);
                GUILayout.Label("Select trail transform");
                GUILayout.BeginHorizontal();
                if (UITools.Button("Bottom"))
                {
                    SelectTrailTransform(Selection.activeGameObject, false);
                }

                if (UITools.Button("Top"))
                {
                    SelectTrailTransform(Selection.activeGameObject, true);
                }
                GUILayout.EndHorizontal();

            }
            UITools.EndSection();


            GUILayout.EndScrollView();
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawGizmos(Whacker whacker, GizmoType gizmoType)
        {
            if (!instance || !instance.ShowWhackerGuides)
            {
                return;
            }

            foreach (Transform t in whacker.transform)
            {
                if (t.name == "LeftSaber")
                {
                    DrawSaberGizmo(t, true);
                }
                else if (t.name == "RightSaber")
                {
                    DrawSaberGizmo(t, false);
                }
            }
        }

        private static void DrawSaberGizmo(Transform t, bool isLeft)
        {
            Gizmos.color = isLeft ? instance.CustomColorLeft : instance.CustomColorRight;
            Gizmos.DrawWireCube(t.position + new Vector3(0, 0, SaberLength / 2 - SaberOffset), new Vector3(0.05f, 0.05f, SaberLength));
            Gizmos.color = Color.white;

            var trails = t.GetComponentsInChildren<Trail>();
            foreach (var trail in trails)
            { 
                DrawTrailGizmo(trail, isLeft);
            }
        }

        private static void DrawTrailGizmo(Trail trail, bool isLeft)
        {
            if (!instance || !instance.ShowTrailGuides)
            {
                return;
            }

            var trailWidth = trail.topTransform.position.z - trail.bottomTransform.position.z;
            var gizmoWidth = trail.Length / 20.0f;

            Gizmos.color = isLeft ? instance.CustomColorLeft : instance.CustomColorRight;
            Gizmos.DrawWireCube(trail.bottomTransform.position + new Vector3(0.025f + gizmoWidth / 2, 0, trailWidth / 2), new Vector3(gizmoWidth, 0.05f, trailWidth));
            Gizmos.color = Color.white;
        }

        private GameObject AddPrefab(Transform t)
        {
            if (!_templatePrefab)
            {
                return null;
            }

            return Instantiate(_templatePrefab, t, false);
        }

        private void FixLength()
        {
            Vector3 Abs(Vector3 vec)
            {
                return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
            }

            foreach (var gameObject in Selection.gameObjects)
            {
                var t = gameObject.transform;
                var localToWorld = t.localToWorldMatrix;
                var worldToLocal = t.worldToLocalMatrix;

                var ogScale = Abs(localToWorld.rotation * t.localScale);
                ogScale.z = 1f;
                t.localScale = Abs(worldToLocal.rotation * ogScale);

                var bounds = GetObjectBounds(gameObject).extents * 2;
                var targetZ = SaberLength / bounds.z;
                ogScale.z = targetZ;
                t.localScale = Abs(worldToLocal.rotation * ogScale);
            }
        }

        public static Bounds GetObjectBounds(GameObject g)
        {
            var b = new Bounds(g.transform.position, Vector3.zero);
            foreach (var r in g.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
            return b;
        }

        private void CreateTemplate()
        {
            var rootGo = new GameObject(_templateText);
            var saberDescriptor = rootGo.AddComponent<Whacker>();
            saberDescriptor.ObjectName = _templateText;

            if (_projectSettings)
            {
                saberDescriptor.Author = _projectSettings.Author;
            }

            var leftSaberGo = new GameObject("LeftSaber");
            leftSaberGo.transform.parent = rootGo.transform;
            CreateTrail(leftSaberGo, Trail.ColorType.Left);
            AddPrefab(leftSaberGo.transform);
            SetCustomColors(leftSaberGo, CustomColorLeft, CustomColorRight);

            if (_createRightSaber)
            {
                leftSaberGo.transform.position = new Vector3(-_spacingBetweenSabers, 0, 0);

                var rightSaberGo = new GameObject("RightSaber");
                rightSaberGo.transform.parent = rootGo.transform;
                CreateTrail(rightSaberGo, Trail.ColorType.Right);
                rightSaberGo.transform.position = new Vector3(_spacingBetweenSabers, 0, 0);

                AddPrefab(rightSaberGo.transform);
                SetCustomColors(rightSaberGo, CustomColorRight, CustomColorLeft);
            }

            Selection.activeGameObject = rootGo;
        }

        private void ColorWhacker(Whacker whacker)
        {
            var leftSaber = whacker.transform.Find("LeftSaber");
            if (leftSaber)
            {
                SetCustomColors(leftSaber.gameObject, CustomColorLeft, CustomColorRight);
            }

            var rightSaber = whacker.transform.Find("RightSaber");
            if (rightSaber)
            {
                SetCustomColors(rightSaber.gameObject, CustomColorRight, CustomColorLeft);
            }
        }

        private void SetCustomColors(GameObject parent, Color thisColor, Color thatColor)
        {
            var renderers = parent.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                Undo.RecordObjects(materials, "Change material Color");
                foreach (var material in materials)
                {
                    if (material.ShouldCC())
                    {
                        if (material.HasProperty("_Color"))
                            material.SetColor("_Color", thisColor);
                        if (material.HasProperty("_OtherColor"))
                            material.SetColor("_OtherColor", thatColor);
                    }
                }
            }
            Undo.IncrementCurrentGroup();
        }

        private void CreateTrail(GameObject saberGo, Trail.ColorType colorType)
        {

            var trailGuides = new GameObject("Trail").transform;
            trailGuides.parent = saberGo.transform;

            var trailGuideTop = new GameObject("Trail Top").transform;
            trailGuideTop.parent = trailGuides;
            trailGuideTop.localPosition = new Vector3(0, 0, SaberLength - SaberOffset);

            var trailGuideBottom = new GameObject("Trail Bottom").transform;
            trailGuideBottom.parent = trailGuides;
            trailGuideBottom.localPosition = new Vector3(0, 0, SaberLength - SaberOffset - _trailWidth);

            var trail = trailGuides.gameObject.AddComponent<Trail>();
            
            trail.topTransform = trailGuideTop;
            trail.bottomTransform = trailGuideBottom;

            trail.trailMaterial = _trailMaterial;
            trail.Length = _trailLength;
            trail.Colortype = colorType;
        }

        private List<GameObject> SelectAllRenderers(GameObject root)
        {
            var gos = new List<GameObject>();
            foreach (var meshRenderer in root.GetComponentsInChildren<Renderer>())
            {
                gos.Add(meshRenderer.gameObject);
            }
            return gos;
        }

        private void SelectTrailTransform(GameObject root, bool top)
        {
            var trails = root.GetComponentsInChildren<Trail>();
            var gos = new List<GameObject>();
            foreach (var trail in trails)
            {
                var t = top ? trail.topTransform : trail.bottomTransform;
                if (t) gos.Add(t.gameObject);
            }

            Selection.objects = gos.ToArray();
        }
    }
}