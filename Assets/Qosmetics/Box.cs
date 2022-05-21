using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Qosmetics.Core;

namespace Qosmetics.Walls
{
    public class Box : MonoBehaviour, IExportable
    {
        [SerializeField]
        private PackageInfo packageJson = new PackageInfo();
        [SerializeField]
        private Qosmetics.Walls.Config config = new Qosmetics.Walls.Config();
        public string ObjectName
        {
            get => packageJson.descriptor.objectName;
            set
            {
                packageJson.androidFileName = $"{value.ToLower()}_android";
                packageJson.pcFileName = $"{value.ToLower()}_pc";
                packageJson.descriptor.objectName = value;
            }
        }
        public string Author
        {
            get => packageJson.descriptor.author;
            set => packageJson.descriptor.author = value;
        }

        public string Description
        {
            get => packageJson.descriptor.description;
            set => packageJson.descriptor.description = value;
        }

        public PackageInfo PackageJson
        {
            get
            {
                packageJson.config = config;
                return packageJson;
            }
        }

        public Texture2D Thumbnail
        {
            get => thumbnail;
            set => thumbnail = value;
        }

        public bool replaceCoreMaterial { get => config.replaceCoreMaterial; set => config.replaceCoreMaterial = value; }
        public bool replaceFrameMaterial { get => config.replaceFrameMaterial; set => config.replaceFrameMaterial = value; }
        public bool replaceCoreMesh { get => config.replaceCoreMesh; set => config.replaceCoreMesh = value; }
        public bool replaceFrameMesh { get => config.replaceFrameMesh; set => config.replaceFrameMesh = value; }
        public bool disableCore { get => config.disableCore; set => config.disableCore = value; }
        public bool disableFrame { get => config.disableFrame; set => config.disableFrame = value; }
        public bool disableFakeGlow { get => config.disableFakeGlow; set => config.disableFakeGlow = value; }

        [SerializeField]
        Transform core = null;
        [SerializeField]
        Transform frame = null;

        [SerializeField]
        public Texture2D thumbnail;

        public string ValidateObject()
        {
            core = transform.Find("Core");
            frame = transform.Find("Frame");

            if ((config.replaceCoreMaterial || config.replaceCoreMesh) && config.disableCore)
                return "Can't replace core material / mesh and also disable core, conflicting options!";
            if ((config.replaceCoreMaterial || config.replaceCoreMesh) && core == null)
                return "Can't replace core material / mesh and not have a core object, please make one!";
            if ((config.replaceFrameMaterial || config.replaceFrameMesh) && config.disableFrame)
                return "Can't replace frame material / mesh and also disable frame, conflicting options!";
            if ((config.replaceFrameMaterial || config.replaceFrameMesh) && frame == null)
                return "Can't replace frame material / mesh and not have a frame object, please make one!";

            var coreRenderer = core?.gameObject.GetComponent<MeshRenderer>();
            if (config.replaceCoreMaterial && (coreRenderer == null || coreRenderer.sharedMaterials.Any((x) => x == null) || coreRenderer.sharedMaterial == null))
                return "can't replace core material if you have no renderer or null materials on the renderer!";
            var coreFilter = core?.gameObject.GetComponent<MeshFilter>();
            if (config.replaceCoreMesh && (coreFilter == null || coreFilter.sharedMesh == null))
                return "can't replace core mesh if you have no meshfilter or no mesh on the meshfilter!";
            var frameRenderer = frame?.gameObject.GetComponent<MeshRenderer>();
            if (config.replaceFrameMaterial && (frameRenderer == null || frameRenderer.sharedMaterials.Any((x) => x == null) || frameRenderer.sharedMaterial == null))
                return "can't replace frame material if you have no renderer or null materials on the renderer!";
            var frameFilter = frame?.gameObject.GetComponent<MeshFilter>();
            if (config.replaceFrameMesh && (frameFilter == null || frameFilter.sharedMesh == null))
                return "can't replace frame mesh if you have no meshfilter or no mesh on the meshfilter!";

            List<MeshRenderer> meshRenderers = new List<MeshRenderer> { };

            if (core != null) meshRenderers.AddRange(core.gameObject.GetComponentsInChildren<MeshRenderer>(true));
            if (frame != null) meshRenderers.AddRange(frame.gameObject.GetComponentsInChildren<MeshRenderer>(true));

            config.isMirrorable = true;
            foreach (var renderer in meshRenderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null) continue;
                    if (!material.HasProperty("_Alpha")) config.isMirrorable = false;
                    if (!material.HasProperty("_StencilRefID")) config.isMirrorable = false;
                    if (!material.HasProperty("_StencilComp")) config.isMirrorable = false;
                    if (!material.HasProperty("_StencilOp")) config.isMirrorable = false;
                    if (!material.HasProperty("_BlendSrcFactor")) config.isMirrorable = false;
                    if (!material.HasProperty("_BlendDstFactor")) config.isMirrorable = false;
                    if (!material.HasProperty("_BlendSrcFactorA")) config.isMirrorable = false;
                    if (!material.HasProperty("_BlendDstFactorA")) config.isMirrorable = false;
                    if (!config.isMirrorable) break;
                }
                if (!config.isMirrorable) break;
            }
            return "";
        }

        public void OnExport()
        {
            var SizeParams = GetComponent<sizeParamsScript>();
            if (SizeParams) Object.DestroyImmediate(SizeParams);
        }
    }

    [System.Serializable]
    public class Config : Qosmetics.Core.Config
    {
        public bool replaceCoreMaterial = true;
        public bool replaceFrameMaterial = false;
        public bool replaceCoreMesh = false;
        public bool replaceFrameMesh = false;
        public bool disableCore = false;
        public bool disableFrame = false;
        public bool disableFakeGlow = true;
        public bool isMirrorable;

    }
}