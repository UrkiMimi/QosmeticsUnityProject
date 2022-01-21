using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                string fileName = Path.GetFileName(path);
                string folderPath = Path.GetDirectoryName(path);

                PackageInfo packageJson = gameObject.GetComponent<IExportable>().PackageJson;

                string androidFileName = packageJson.androidFileName;
                string pcFileName = packageJson.pcFileName;

                Selection.activeObject = gameObject;
                EditorUtility.SetDirty(gameObject);
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
                EditorSceneManager.SaveScene(gameObject.scene);

                PrefabUtility.SaveAsPrefabAsset(Selection.activeObject as GameObject, $"Assets/Qosmetics/{prefabName}.prefab");

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
    }
}
