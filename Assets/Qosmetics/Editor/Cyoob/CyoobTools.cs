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
        bool _isBeatSaberLookOpen = false;
        bool _isOtherToolsOpen = false;

        string _templateText = "";
        GameObject _arrowTemplate = null;
        GameObject _dotTemplate = null;
        GameObject _bombTemplate = null;
        GameObject _headTemplate = null;
        GameObject _linkTemplate = null;

        GameObject _debrisTemplate = null;
        GameObject _linkDebrisTemplate = null;
        GameObject _headDebrisTemplate = null;

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
                GUILayout.Label("Debris");
                GUILayout.BeginVertical("box");
                _debrisTemplate = (GameObject)EditorGUILayout.ObjectField("Debris Prefab", _debrisTemplate, typeof(GameObject), false);
                if (_headTemplate != null) _headDebrisTemplate = (GameObject)EditorGUILayout.ObjectField("Chain Head Debris Prefab", _headDebrisTemplate, typeof(GameObject), false);
                if (_linkTemplate != null) _linkDebrisTemplate = (GameObject)EditorGUILayout.ObjectField("Chain Link Debris Prefab", _linkDebrisTemplate, typeof(GameObject), false);
                GUILayout.EndVertical();
                GUILayout.Space(5);
                if (GUILayout.Button("Create Template", GUILayout.Height(20)))
                {
                    CreateTemplate();
                }
            }
            UITools.EndSection();
            /*
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
            */
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
                    Selection.objects = ExporterUtils.SelectAllRenderers(Selection.activeGameObject).ToArray();
                }

                if (UITools.Button("Detect Mirrorable"))
                {
                    ExporterUtils.DetectMirrorable(_selectedCyoob?.gameObject);
                }

                if (UITools.Button("Layout Cyoob"))
                {
                    LayoutCyoobs();
                }

                if (UITools.Button("Zero out cyoob"))
                {
                    ReturnCyoobs();
                }
            }
            UITools.EndSection();


            GUILayout.EndScrollView();
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawGizmos(Cyoob cyoob, GizmoType gizmoType)
        {
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


        static Color ColorLerp(Color a, Color b, float t)
        {
            return a * (1.0f - t) + b * t;
        }
        static void DrawNotesGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                Gizmos.color = ColorLerp(Gizmos.color, Color.white, .5f);
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
                Gizmos.color = ColorLerp(Gizmos.color, Color.white, .5f);
                Gizmos.DrawWireCube(child.position, Vector3.one * cyoobSize);
            }
        }
        static void DrawChainGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                Gizmos.color = ColorLerp(Gizmos.color, Color.white, .5f);
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
                Gizmos.color = ColorLerp(Gizmos.color, Color.white, .5f);
                Gizmos.DrawWireCube(child.position, new Vector3(cyoobSize, 0.75f * cyoobSize, cyoobSize));
            }
        }

        static void DrawChainLinkDebrisGizmo(Transform t)
        {
            foreach (Transform child in t)
            {
                Gizmos.color = child.name.StartsWith("Left") ? instance.CustomColorLeft : instance.CustomColorRight;
                Gizmos.color = ColorLerp(Gizmos.color, Color.white, .5f);
                Gizmos.DrawWireCube(child.position, new Vector3(cyoobSize, 0.2f * cyoobSize, cyoobSize));
            }
        }

        GameObject AddPrefab(GameObject prefab, Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.parent = parent;
            return Object.Instantiate(prefab, parent, false);
        }

        public void CreateTemplate()
        {
            if (!_arrowTemplate)
            {
                Debug.LogError("Can't make template, Arrow Prefab was not given!");
                return;
            }

            if (!_dotTemplate)
            {
                Debug.LogError("Can't make template, Dot Prefab was not given!");
                return;
            }

            if (_headTemplate ^ _linkTemplate)
            {
                Debug.LogError("Can't make template, the chain head or link was not given!");
                return;
            }


            var rootGo = new GameObject(_templateText);
            var cyoob = rootGo.AddComponent<Cyoob>();
            cyoob.ObjectName = _templateText;

            var leftObjects = new List<GameObject>();
            var rightObjects = new List<GameObject>();

            if (_projectSettings)
            {
                cyoob.Author = _projectSettings.Author;
            }

            var notes = new GameObject("Notes");
            notes.transform.parent = rootGo.transform;

            leftObjects.Add(AddPrefab(_arrowTemplate, notes.transform, "LeftArrow"));
            rightObjects.Add(AddPrefab(_arrowTemplate, notes.transform, "RightArrow"));

            leftObjects.Add(AddPrefab(_dotTemplate, notes.transform, "LeftDot"));
            rightObjects.Add(AddPrefab(_dotTemplate, notes.transform, "RightDot"));

            if (_bombTemplate)
                AddPrefab(_bombTemplate, rootGo.transform, "Bomb");

            if (_headTemplate)
            {
                var chains = new GameObject("Chains");
                chains.transform.parent = rootGo.transform;
                leftObjects.Add(AddPrefab(_headTemplate, chains.transform, "LeftHead"));
                rightObjects.Add(AddPrefab(_headTemplate, chains.transform, "RightHead"));

                leftObjects.Add(AddPrefab(_linkTemplate, chains.transform, "LeftLink"));
                rightObjects.Add(AddPrefab(_linkTemplate, chains.transform, "RightLink"));
            }

            if (_debrisTemplate)
            {
                var debris = new GameObject("Debris");
                debris.transform.parent = rootGo.transform;
                leftObjects.Add(AddPrefab(_debrisTemplate, debris.transform, "LeftDebris"));
                rightObjects.Add(AddPrefab(_debrisTemplate, debris.transform, "RightDebris"));
            }

            if (_headTemplate && _headDebrisTemplate)
            {
                var debris = new GameObject("ChainHeadDebris");
                debris.transform.parent = rootGo.transform;
                leftObjects.Add(AddPrefab(_headDebrisTemplate, debris.transform, "LeftDebris"));
                rightObjects.Add(AddPrefab(_headDebrisTemplate, debris.transform, "RightDebris"));
            }

            if (_linkTemplate && _linkDebrisTemplate)
            {
                var debris = new GameObject("ChainLinkDebris");
                debris.transform.parent = rootGo.transform;
                leftObjects.Add(AddPrefab(_linkDebrisTemplate, debris.transform, "LeftDebris"));
                rightObjects.Add(AddPrefab(_linkDebrisTemplate, debris.transform, "RightDebris"));
            }

            foreach (var leftObject in leftObjects) SetCustomColors(leftObject, CustomColorLeft, CustomColorRight);
            foreach (var rightObject in rightObjects) SetCustomColors(rightObject, CustomColorRight, CustomColorLeft);
        }

        public static Bounds GetObjectBounds(GameObject g)
        {
            var b = new Bounds(g.transform.position, Vector3.zero);
            foreach (var r in g.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
            return b;
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
        bool RecursiveParentSearch(Transform child, Transform target)
        {
            if (child == null)
                return false;
            if (child == target)
                return true;
            return RecursiveParentSearch(child.parent, target);
        }

        void ClampSize(GameObject obj, Vector3 target)
        {
            if (!RecursiveParentSearch(Selection.activeTransform, obj.transform))
                return;
            Vector3 Abs(Vector3 vec)
            {
                return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
            }
            var renderers = ExporterUtils.SelectAllRenderers(obj);

            foreach (var renderer in renderers)
            {
                var t = renderer.transform;
                var localToWorld = t.localToWorldMatrix;
                var worldToLocal = t.worldToLocalMatrix;

                var ogScale = Abs(localToWorld.rotation * t.localScale);
                var actualBounds = GetObjectBounds(obj).extents * 2;
                var largestFactor = Mathf.Max(actualBounds.x, actualBounds.y, actualBounds.z);

                if (actualBounds.x > target.x || actualBounds.y > target.y || actualBounds.z > target.z)
                {
                    t.localScale = ogScale / largestFactor;
                }
            }
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

        private void ReturnCyoobs()
        {
            var cyoob = _selectedCyoob;
            if (!cyoob) return;

            var valid = cyoob.ValidateObject();
            if (string.IsNullOrEmpty(valid))
            {
                cyoob.leftArrow.transform.localPosition = Vector3.zero;
                cyoob.leftDot.transform.localPosition = Vector3.zero;
                cyoob.rightArrow.transform.localPosition = Vector3.zero;
                cyoob.rightDot.transform.localPosition = Vector3.zero;

                if (cyoob.leftHead) cyoob.leftHead.transform.localPosition = Vector3.zero;
                if (cyoob.leftLink) cyoob.leftLink.transform.localPosition = Vector3.zero;
                if (cyoob.rightHead) cyoob.rightHead.transform.localPosition = Vector3.zero;
                if (cyoob.rightLink) cyoob.rightLink.transform.localPosition = Vector3.zero;

                if (cyoob.bomb) cyoob.bomb.transform.localPosition = Vector3.zero;

                if (cyoob.leftDebris) cyoob.leftDebris.transform.localPosition = Vector3.zero;
                if (cyoob.rightDebris) cyoob.rightDebris.transform.localPosition = Vector3.zero;

                if (cyoob.leftChainHeadDebris) cyoob.leftChainHeadDebris.transform.localPosition = Vector3.zero;
                if (cyoob.rightChainHeadDebris) cyoob.rightChainHeadDebris.transform.localPosition = Vector3.zero;

                if (cyoob.leftChainLinkDebris) cyoob.leftChainLinkDebris.transform.localPosition = Vector3.zero;
                if (cyoob.rightChainLinkDebris) cyoob.rightChainLinkDebris.transform.localPosition = Vector3.zero;

            }
            else
                EditorUtility.DisplayDialog("Return Cyoobs Failed", valid, "OK");
        }
        private void LayoutCyoobs()
        {
            var cyoob = _selectedCyoob;
            if (!cyoob) return;

            var valid = cyoob.ValidateObject();
            if (string.IsNullOrEmpty(valid))
            {
                const float unit = 1.25f;
                cyoob.leftArrow.transform.localPosition = new Vector3(0, unit, 0);
                cyoob.leftDot.transform.localPosition = new Vector3(0, 0, 0);
                cyoob.rightArrow.transform.localPosition = new Vector3(unit, unit, 0);
                cyoob.rightDot.transform.localPosition = new Vector3(unit, 0, 0);

                if (cyoob.leftHead) cyoob.leftHead.transform.localPosition = new Vector3(-unit * 2, unit, 0);
                if (cyoob.leftLink) cyoob.leftLink.transform.localPosition = new Vector3(-unit * 2, 0, 0);
                if (cyoob.rightHead) cyoob.rightHead.transform.localPosition = new Vector3(-unit, unit, 0);
                if (cyoob.rightLink) cyoob.rightLink.transform.localPosition = new Vector3(-unit, 0, 0);

                if (cyoob.bomb) cyoob.bomb.transform.localPosition = new Vector3(unit * 2, 0, 0);

                if (cyoob.leftDebris) cyoob.leftDebris.transform.localPosition = new Vector3(0, -unit, 0);
                if (cyoob.rightDebris) cyoob.rightDebris.transform.localPosition = new Vector3(unit, -unit, 0);

                if (cyoob.leftChainHeadDebris) cyoob.leftChainHeadDebris.transform.localPosition = new Vector3(-unit * 2, -unit, 0);
                if (cyoob.rightChainHeadDebris) cyoob.rightChainHeadDebris.transform.localPosition = new Vector3(-unit, -unit, 0);

                if (cyoob.leftChainLinkDebris) cyoob.leftChainLinkDebris.transform.localPosition = new Vector3(-unit * 2, -unit * 2, 0);
                if (cyoob.rightChainLinkDebris) cyoob.rightChainLinkDebris.transform.localPosition = new Vector3(-unit, -unit * 2, 0);

            }
            else
                EditorUtility.DisplayDialog("Layout Cyoobs Failed", valid, "OK");
        }
    }
}