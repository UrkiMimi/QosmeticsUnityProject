using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.IO.Compression;
using UnityEditor.SceneManagement;
using System;

namespace Qosmetics.Core
{
    public static class ExporterUtils
    {
        static string WorkingDir { get => $"{Application.temporaryCachePath}/Qosmetics/"; }

        static bool exporting = false;
        /// <summary>
        /// exports a prefab with packagejson as prefabName
        /// </summary>
        /// <param name="gameObject"></param> the gameobject to turn into a prefab
        /// <param name="prefabName"></param> the name of the prefab in the asset bundle
        /// <param name="path"></param> the path to which to export this to
        public static void ExportAsPrefabPackage(this GameObject gameObject, string prefabName, string path, Texture2D thumbnail = null)
        {
            if (exporting) {
                Debug.LogError("Already exporting a package, not exporting another one!");
                return;
            }
            exporting = true;
            try
            {
                GameObject toExport = UnityEngine.Object.Instantiate(gameObject);
                string fileName = Path.GetFileName(path);
                string folderPath = Path.GetDirectoryName(path);

                IExportable exportable = toExport.GetComponent<IExportable>();
                PackageInfo packageJson = exportable.PackageJson;

                string androidFileName = packageJson.androidFileName;
                string pcFileName = packageJson.pcFileName;
                exportable.OnExport();

                UnityEngine.Object.DestroyImmediate(exportable as MonoBehaviour);
                EditorUtility.SetDirty(toExport);
                EditorSceneManager.MarkSceneDirty(toExport.scene);
                EditorSceneManager.SaveScene(toExport.scene);

                PrefabUtility.SaveAsPrefabAsset(toExport, $"Assets/Qosmetics/{prefabName}.prefab");
                UnityEngine.Object.DestroyImmediate(toExport.gameObject);

                AssetBundleBuild assetBundleBuild = default(AssetBundleBuild);

                assetBundleBuild.assetNames = new string[]
                {
                    $"Assets/Qosmetics/{prefabName}.prefab"
                };

                Directory.CreateDirectory(WorkingDir);
                Export(assetBundleBuild, $"{WorkingDir}/{pcFileName}", BuildTarget.StandaloneWindows64);
                Export(assetBundleBuild, $"{WorkingDir}/{androidFileName}", BuildTarget.Android);
                File.WriteAllText($"{WorkingDir}/package.json", packageJson.ToJson());

                List<string> files = new List<string>()
                {
                    $"{WorkingDir}/{pcFileName}",
                    $"{WorkingDir}/{androidFileName}",
                    $"{WorkingDir}/package.json"
                };

                // if thumbnail given, add it to the files
                if (thumbnail)
                {
                    var assetPath = AssetDatabase.GetAssetPath(thumbnail);
                    if (string.IsNullOrEmpty(assetPath))
                        File.WriteAllBytes($"{WorkingDir}/thumbnail.png", thumbnail.EncodeToPNG());
                    else
                        File.Copy(Application.dataPath + assetPath.Substring(6), $"{WorkingDir}/thumbnail.png");
                    files.Add($"{WorkingDir}/thumbnail.png");
                }

                if (File.Exists($"{WorkingDir}/tempzip.zip"))
                    File.Delete($"{WorkingDir}/tempzip.zip");
                CreateZipFile($"{WorkingDir}/tempzip.zip", files.ToArray());

                if (File.Exists(path)) File.Delete(path);

                foreach (var file in files) if (File.Exists(file)) File.Delete(file);

                File.Move($"{WorkingDir}/tempzip.zip", path);

                // cleanup
                AssetDatabase.DeleteAsset($"Assets/Qosmetics/{prefabName}.prefab");
                Directory.Delete(WorkingDir, true);
                AssetDatabase.Refresh();
                exporting = false;

                EditorUtility.DisplayDialog("Exportation successful!", "Exportation successful!", "OK");
                EditorUtility.RevealInFinder(path);
            }
            finally
            {
                exporting = false;
            }
        }

        static void Export(AssetBundleBuild assetBundleBuild, string path, BuildTarget target)
        {
            assetBundleBuild.assetBundleName = Path.GetFileName(path);
            BuildPipeline.BuildAssetBundles(Path.GetDirectoryName(path), new AssetBundleBuild[] { assetBundleBuild }, 0, target);
        }

        public static void CreateZipFile(string fileName, IEnumerable<string> files)
        {
            // Create and open a new ZIP file
            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), System.IO.Compression.CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }

        public static void CompletelyUnpackPrefab(this Transform root)
        {
            if (PrefabUtility.GetPrefabInstanceStatus(root) != PrefabInstanceStatus.NotAPrefab)
                PrefabUtility.UnpackPrefabInstance(PrefabUtility.GetOutermostPrefabInstanceRoot(root), PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            for (int i = 0; i < root.childCount; i++)
            {
                root.GetChild(i).CompletelyUnpackPrefab();
            }
        }

        public static bool ShouldCC(this Material mat)
        {
            if (!mat) return false;
            else if (mat.HasProperty("_CustomColors"))
                return mat.GetFloat("_CustomColors") > 0;
            else if (mat.HasProperty("_Glow"))
                return mat.GetFloat("_Glow") > 0;
            else if (mat.HasProperty("_Bloom"))
                return mat.GetFloat("_Bloom") > 0;
            return false;
        }

        public static List<GameObject> SelectAllRenderers(GameObject root)
        {
            var gos = new List<GameObject>();
            if (root)
            {
                foreach (var meshRenderer in root.GetComponentsInChildren<Renderer>(true)) gos.Add(meshRenderer.gameObject);
            }
            return gos;
        }

        public static void DetectMirrorable(GameObject root)
        {
            if (root)
            {
                var renderers = root.gameObject.GetComponentsInChildren<MeshRenderer>();
                Material badMaterial = null;
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (material == null) continue;
                        if (!material.HasProperty("_Alpha")) badMaterial = material;
                        if (!material.HasProperty("_StencilRefID")) badMaterial = material;
                        if (!material.HasProperty("_StencilComp")) badMaterial = material;
                        if (!material.HasProperty("_StencilOp")) badMaterial = material;
                        if (!material.HasProperty("_BlendSrcFactor")) badMaterial = material;
                        if (!material.HasProperty("_BlendDstFactor")) badMaterial = material;
                        if (!material.HasProperty("_BlendSrcFactorA")) badMaterial = material;
                        if (!material.HasProperty("_BlendDstFactorA")) badMaterial = material;
                        // if we find a bad material, break
                        if (badMaterial) break;
                    }
                }

                if (!badMaterial)
                    EditorUtility.DisplayDialog("Object mirrorable", "This object was mirrorable!", "OK");
                else
                    EditorUtility.DisplayDialog("Object not mirrorable", $"This object is not (fully) mirrorable!, material {AssetDatabase.GetAssetPath(badMaterial)} is missing the correct properties", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Nothing selected", "You do not have anything selected that has a qosmetics component on the object or it's parents, can't verify mirrorability.", "OK");
            }
        }

        public static string GenerateThumbnail()
        {
            var cam = Qosmetics.Core.ThumbnailCamera.Instance;
            var path = EditorUtility.SaveFilePanel("Save thumbnail", Application.dataPath, "thumbnail.png", "png");
            if (string.IsNullOrEmpty(path) || !path.Contains(Application.dataPath))
                return "";

            var thumbnail = cam.Generate();

            var data = thumbnail.EncodeToPNG();
            File.WriteAllBytes(path, data);
            AssetDatabase.Refresh();

            return path.Substring(Application.dataPath.Length - 6);
        }

        public static void ObservedObjectField<T>(T OriginalObject, string label, bool allowSceneObjects, Action<T> onChange) where T : UnityEngine.Object {
            var oldObject = OriginalObject;
            var newObject = (T)EditorGUILayout.ObjectField(label, OriginalObject, typeof(T), allowSceneObjects);
            if (oldObject != newObject) {
                onChange?.Invoke(newObject);
            }
        }

        public static void ObservedColorField(Color originalColor, string label, Action<Color> onChange) {
            var oldColor = originalColor;
            var newColor = EditorGUILayout.ColorField(label, originalColor);
            if (oldColor != newColor) {
                onChange?.Invoke(newColor);
            }
        }

        public static void ObservedIntField(int originalInt, string label, Action<int> onChange) {
            var oldInt = originalInt;
            var newInt = EditorGUILayout.IntField(label, originalInt);
            if (oldInt != newInt) {
                onChange?.Invoke(newInt);
            }
        }

        public static void ObservedSlider(float originalFloat, string label, float min, float max, Action<float> onChange) {
            var oldFloat = originalFloat;
            var newFloat = EditorGUILayout.Slider(label, originalFloat, min, max);
            if (oldFloat != newFloat) {
                onChange?.Invoke(newFloat);
            }
        }

        public static void ObservedStringField(string originalString, string label, Action<string> onChange) {
            var oldString = originalString;
            var newString = EditorGUILayout.TextField(label, originalString);
            if (oldString != newString) {
                onChange?.Invoke(newString);
            }
        }
    }
}
