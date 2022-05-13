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
        public static void ExportAsPrefabPackage(this GameObject gameObject, string prefabName, string path)
        {
            if (exporting) return;
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

                string[] files =
                {
                $"{WorkingDir}/{pcFileName}",
                $"{WorkingDir}/{androidFileName}",
                $"{WorkingDir}/package.json"
                };

                if (File.Exists($"{WorkingDir}/tempzip.zip"))
                    File.Delete($"{WorkingDir}/tempzip.zip");
                CreateZipFile($"{WorkingDir}/tempzip.zip", files);

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
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
            }
        }

        static void Export(AssetBundleBuild assetBundleBuild, string path, BuildTarget target)
        {
            assetBundleBuild.assetBundleName = Path.GetFileName(path);
            BuildPipeline.BuildAssetBundles(Path.GetDirectoryName(path), new AssetBundleBuild[] { assetBundleBuild}, 0, target);
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
    }
}
