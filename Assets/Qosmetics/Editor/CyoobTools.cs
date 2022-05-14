using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qosmetics.Core;
using UnityEditor;

namespace Qosmetics.Notes
{
    public class CyoobTools : EditorWindow
    {
        static public CyoobTools instance;

        static float cyoobSize = 1.0f;
        private Vector2 _scrollPos = Vector2.zero;

        public bool ShowCyoobGuides = true;

        public bool PreviewCC = true;
        public bool BeatSaberLookActive;

        public Color CustomColorLeft = Color.red;
        public Color CustomColorRight = Color.cyan;

        QosmeticsProjectSettings _projectSettings;
        Cyoob _selectedCyoob = null;

        bool _isGuidesOpen = false;
        bool _isCreateSaberOpen = false;
        bool _isFixingOpen = false;
        bool _isBeatSaberLookOpen = false;
        bool _isOtherToolsOpen = false;

        string _templateText = "";
        GameObject _arrowTemplate = null;
        GameObject _dotTemplate = null;
        GameObject _bombTemplate = null;
        GameObject _headTemplate = null;
        GameObject _linkTemplate = null;

        [MenuItem("Qosmetics/Cyoob Tools")]
        public static void OpenNoteTools()
        {
            instance = GetWindow<CyoobTools>(false, "Cyoob Tools");
        }

        Theme _theme;
        public void OnFocus()
        {
            _projectSettings = QosmeticsProjectSettings.GetOrCreateSettings();
            _theme = Theme.GetTheme();
        }

        public void GetSelectedCyoob()
        {
            if (Selection.activeGameObject)
            {
                _selectedCyoob = Selection.activeGameObject.GetComponent<Cyoob>();
            }
            else
            {
                _selectedCyoob = null;
            }
        }

        public void OnGUI()
        {
            return;
            GetSelectedCyoob();
            GUILayout.Space(10);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Visualizers", _theme.HeaderColor);
            UITools.Foldout(ref _isGuidesOpen);
            if (_isGuidesOpen)
            {
                UITools.ChangedToggle(ref ShowCyoobGuides, "Show Guides", val =>
                {
                    SceneView.RepaintAll();
                });
            }
            UITools.EndSection();

            UITools.BeginSection(_theme.BackgroundColor);
            UITools.CenterHeader("Create Cyoob", _theme.HeaderColor);
            UITools.Foldout(ref _isCreateSaberOpen);
            if (_isCreateSaberOpen)
            {
                UITools.Header("General", Color.cyan);
                _templateText = EditorGUILayout.TextField("Name", _templateText);
                GUILayout.Space(2);
                GUILayout.Label("Required");
                GUILayout.BeginVertical("box");
                _arrowTemplate = (GameObject)EditorGUILayout.ObjectField("Arrow Prefab", _arrowTemplate, typeof(GameObject), false);
                _dotTemplate = (GameObject)EditorGUILayout.ObjectField("Dot Prefab", _dotTemplate, typeof(GameObject), false);
                GUILayout.EndVertical();
                GUILayout.Label("Optional");
                GUILayout.BeginVertical("box");
                _bombTemplate = (GameObject)EditorGUILayout.ObjectField("Bomb Prefab", _bombTemplate, typeof(GameObject), false);
                _headTemplate = (GameObject)EditorGUILayout.ObjectField("Chain Head Prefab", _headTemplate, typeof(GameObject), false);
                _linkTemplate = (GameObject)EditorGUILayout.ObjectField("Chain Link Prefab", _linkTemplate, typeof(GameObject), false);
                GUILayout.EndVertical();
                GUILayout.Space(5);
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
                if (UITools.Button("Fix Size"))
                {
                    FixSize();
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
                        var cyoob = Selection.activeGameObject.GetComponent<Cyoob>();
                        if (!cyoob)
                        {
                            cyoob = Selection.activeGameObject.GetComponentInParent<Cyoob>();
                        }

                        if (cyoob) ColorCyoob(cyoob);
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
            }
            UITools.EndSection();


            GUILayout.EndScrollView();
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawGizmos(Cyoob cyoob, GizmoType gizmoType)
        {
            return;
            if (!instance || !instance.ShowCyoobGuides)
            {
                return;
            }

            foreach (Transform t in cyoob.transform)
            {
                if (t.name == "Notes")
                {
                    DrawNotesGizmo(t);
                }
                else if (t.name == "Bomb")
                {
                    DrawBombGizmo(t);
                }
                else if (t.name == "Debris")
                {
                    DrawDebrisGizmo(t);
                }
                else if (t.name == "Chains")
                {
                    DrawChainGizmo(t);
                }
                else if (t.name == "ChainHeadDebris")
                {
                    DrawChainHeadDebrisGizmo(t);
                }
                else if (t.name == "ChainLinkDebris")
                {
                    DrawChainLinkDebrisGizmo(t);
                }
            }
            Gizmos.color = Color.white;
        }


        static void DrawNotesGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                Gizmos.DrawWireCube(child.position, Vector3.one * cyoobSize);
            }
        }
        static void DrawBombGizmo(Transform t)
        {
            Gizmos.color = Color.black * .75f;
            Gizmos.DrawWireCube(t.position, Vector3.one * cyoobSize);
        }
        static void DrawDebrisGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                Gizmos.DrawWireCube(child.position, Vector3.one * cyoobSize);
            }
        }
        static void DrawChainGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                if (child.name.EndsWith("Head"))
                    Gizmos.DrawWireCube(child.position, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));
                else
                    Gizmos.DrawWireCube(child.position, new Vector3(cyoobSize, 0.2f * cyoobSize, cyoobSize));
            }
        }
        static void DrawChainHeadDebrisGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                Gizmos.DrawWireCube(child.position, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));
            }
        }

        static void DrawChainLinkDebrisGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                Gizmos.DrawWireCube(child.position, new Vector3(cyoobSize, 0.2f * cyoobSize, cyoobSize));
            }
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

        public void CreateTemplate()
        {
            Debug.LogError("TODO");
        }

        public static Bounds GetObjectBounds(GameObject g)
        {
            var b = new Bounds(g.transform.position, Vector3.zero);
            foreach (var r in g.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
            return b;
        }
        Vector3 Abs(Vector3 vec)
        {
            return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
        }

        public void FixSize()
        {
            var cyoob = Selection.activeGameObject.GetComponent<Cyoob>();
            if (!cyoob)
            {
                cyoob = Selection.activeGameObject.GetComponentInParent<Cyoob>();
            }

            var leftArrow = cyoob.transform.Find("Notes/LeftArrow")?.gameObject;
            if (leftArrow) ClampSize(leftArrow, new Vector3(cyoobSize, cyoobSize, cyoobSize));
            var rightArrow = cyoob.transform.Find("Notes/RightArrow")?.gameObject;
            if (rightArrow) ClampSize(rightArrow, new Vector3(cyoobSize, cyoobSize, cyoobSize));
            var leftDot = cyoob.transform.Find("Notes/LeftDot")?.gameObject;
            if (leftDot) ClampSize(leftDot, new Vector3(cyoobSize, cyoobSize, cyoobSize));
            var rightDot = cyoob.transform.Find("Notes/RightDot")?.gameObject;
            if (rightDot) ClampSize(rightDot, new Vector3(cyoobSize, cyoobSize, cyoobSize));

            var leftHead = cyoob.transform.Find("Chains/LeftHead")?.gameObject;
            if (leftHead) ClampSize(leftHead, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));
            var rightHead = cyoob.transform.Find("Chains/RightHead")?.gameObject;
            if (rightHead) ClampSize(rightHead, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));
            var leftLink = cyoob.transform.Find("Chains/LeftLink")?.gameObject;
            if (leftLink) ClampSize(leftLink, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));
            var rightLink = cyoob.transform.Find("Chains/RightLink")?.gameObject;
            if (rightLink) ClampSize(rightLink, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));

            var bomb = cyoob.transform.Find("Bomb")?.gameObject;
            if (bomb) ClampSize(bomb, new Vector3(cyoobSize, cyoobSize, cyoobSize));

            var leftDebris = cyoob.transform.Find("Debris/LeftDebris")?.gameObject;
            if (leftDebris) ClampSize(leftDebris, new Vector3(cyoobSize, cyoobSize, cyoobSize));
            var rightDebris = cyoob.transform.Find("Debris/RightDebris")?.gameObject;
            if (rightDebris) ClampSize(rightDebris, new Vector3(cyoobSize, cyoobSize, cyoobSize));

            var leftHeadDebris = cyoob.transform.Find("ChainHeadDebris/LeftDebris")?.gameObject;
            if (leftHeadDebris) ClampSize(leftHeadDebris, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));
            var rightHeadDebris = cyoob.transform.Find("ChainHeadDebris/RightDebris")?.gameObject;
            if (rightHeadDebris) ClampSize(rightHeadDebris, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));

            var leftLinkDebris = cyoob.transform.Find("ChainLinkDebris/LeftDebris")?.gameObject;
            if (leftLinkDebris) ClampSize(leftLinkDebris, new Vector3(cyoobSize, 0.2f * cyoobSize, cyoobSize));
            var rightLinkDebris = cyoob.transform.Find("ChainLinkDebris/RightDebris")?.gameObject;
            if (rightLinkDebris) ClampSize(rightLinkDebris, new Vector3(cyoobSize, 0.2f * cyoobSize, cyoobSize));
        }

        void ClampSize(GameObject obj, Vector3 target) 
        {
            if (obj != Selection.activeGameObject)
                return;
            var t = obj.transform;
            var localToWorld = t.localToWorldMatrix;
            var worldToLocal = t.worldToLocalMatrix;

            var ogScale = Abs(localToWorld.rotation * t.localScale);
            t.localScale = Abs(worldToLocal.rotation * Vector3.one);

            var bounds = GetObjectBounds(obj).extents * 2;
            ogScale = new Vector3(target.x / bounds.x, target.y / bounds.y, target.z / bounds.z);
            t.localScale = Abs(worldToLocal.rotation * ogScale);
        } 

        public void ColorCyoob(Cyoob cyoob)
        {
            var leftObjects = new List<GameObject> { };
            var rightObjects = new List<GameObject> { };

            var leftArrow = cyoob.transform.Find("Notes/LeftArrow")?.gameObject;
            if (leftArrow) leftObjects.Add(leftArrow);
            var rightArrow = cyoob.transform.Find("Notes/RightArrow")?.gameObject;
            if (rightArrow) rightObjects.Add(rightArrow);
            var leftDot = cyoob.transform.Find("Notes/LeftDot")?.gameObject;
            if (leftDot) leftObjects.Add(leftDot);
            var rightDot = cyoob.transform.Find("Notes/RightDot")?.gameObject;
            if (rightDot) rightObjects.Add(rightDot);

            var leftHead = cyoob.transform.Find("Chains/LeftHead")?.gameObject;
            if (leftHead) leftObjects.Add(leftHead);
            var rightHead = cyoob.transform.Find("Chains/RightHead")?.gameObject;
            if (rightHead) rightObjects.Add(rightHead);
            var leftLink = cyoob.transform.Find("Chains/LeftLink")?.gameObject;
            if (leftLink) leftObjects.Add(leftLink);
            var rightLink = cyoob.transform.Find("Chains/RightLink")?.gameObject;
            if (rightLink) rightObjects.Add(rightLink);

            var bomb = cyoob.transform.Find("Bomb")?.gameObject;

            var leftDebris = cyoob.transform.Find("Debris/LeftDebris")?.gameObject;
            if (leftDebris) leftObjects.Add(leftDebris);
            var rightDebris = cyoob.transform.Find("Debris/RightDebris")?.gameObject;
            if (rightDebris) rightObjects.Add(rightDebris);

            var leftHeadDebris = cyoob.transform.Find("ChainHeadDebris/LeftDebris")?.gameObject;
            if (leftHeadDebris) leftObjects.Add(leftHeadDebris);
            var rightHeadDebris = cyoob.transform.Find("ChainHeadDebris/RightDebris")?.gameObject;
            if (rightHeadDebris) rightObjects.Add(rightHeadDebris);

            var leftLinkDebris = cyoob.transform.Find("ChainLinkDebris/LeftDebris")?.gameObject;
            if (leftLinkDebris) leftObjects.Add(leftLinkDebris);
            var rightLinkDebris = cyoob.transform.Find("ChainLinkDebris/RightDebris")?.gameObject;
            if (rightLinkDebris) rightObjects.Add(rightLinkDebris);
            
            foreach (var leftObject in leftObjects)
            {
                SetCustomColors(leftObject, CustomColorLeft, CustomColorRight);
            }
            
            foreach (var rightObject in rightObjects)
            {
                SetCustomColors(rightObject, CustomColorRight, CustomColorLeft);

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
    }
}