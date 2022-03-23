using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qosmetics.Core;

namespace Qosmetics.Notes
{
    public class Cyoob : MonoBehaviour, IExportable
    {
        [SerializeField]
        private PackageInfo packageJson = new PackageInfo();
        [SerializeField]
        private Qosmetics.Notes.Config config = new Qosmetics.Notes.Config();
        public string ObjectName {
            get => packageJson.descriptor.objectName;
            set {
                packageJson.androidFileName = $"{value.ToLower()}_android";
                packageJson.pcFileName = $"{value.ToLower()}_pc";
                packageJson.descriptor.objectName = value;
            } }
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

        public PackageInfo PackageJson { get 
                {
                packageJson.config = config;
                return packageJson;
            } }

        public bool ShowArrows { get => config.showArrows; set => config.showArrows = value; }
        [SerializeField]
        private GameObject leftArrow;
        [SerializeField]
        private GameObject rightArrow;
        [SerializeField]
        private GameObject leftDot;
        [SerializeField]
        private GameObject rightDot;
        [SerializeField]
        private GameObject bomb;
        [SerializeField]
        private GameObject leftDebris;
        [SerializeField]
        private GameObject rightDebris;
        [SerializeField]
        private GameObject leftChainHeadDebris;
        [SerializeField]
        private GameObject rightChainHeadDebris;
        [SerializeField]
        private GameObject leftChainLinkDebris;
        [SerializeField]
        private GameObject rightChainLinkDebris;
        [SerializeField]
        private GameObject leftHead;
        [SerializeField]
        private GameObject rightHead;
        [SerializeField]
        private GameObject leftLink;
        [SerializeField]
        private GameObject rightLink;

        public bool ValidateObject()
        {
            leftArrow = transform.Find("Notes/LeftArrow")?.gameObject;
            rightArrow = transform.Find("Notes/RightArrow")?.gameObject;
            leftDot= transform.Find("Notes/LeftDot")?.gameObject;
            rightDot = transform.Find("Notes/RightDot")?.gameObject;

            bomb = transform.Find("Bomb")?.gameObject;

            leftDebris = transform.Find("Debris/LeftDebris")?.gameObject;
            rightDebris = transform.Find("Debris/RightDebris")?.gameObject;
            leftChainHeadDebris = transform.Find("ChainHeadDebris/LeftDebris")?.gameObject;
            rightChainHeadDebris = transform.Find("ChainHeadDebris/RightDebris")?.gameObject;
            leftChainLinkDebris = transform.Find("ChainLinkDebris/LeftDebris")?.gameObject;
            rightChainLinkDebris = transform.Find("ChainLinkDebris/RightDebris")?.gameObject;

            leftHead = transform.Find("Chains/LeftHead")?.gameObject;
            rightHead = transform.Find("Chains/RightHead")?.gameObject;
            leftLink = transform.Find("Chains/LeftLink")?.gameObject;
            rightLink= transform.Find("Chains/RightLink")?.gameObject;

            config.hasBomb = bomb != null;
            config.hasDebris = leftDebris != null && rightDebris != null;

            config.hasChainHeadDebris = leftChainHeadDebris != null && rightChainHeadDebris != null;
            config.hasChainLinkDebris = leftChainLinkDebris != null && rightChainLinkDebris != null;
            config.hasSlider = leftHead != null && rightHead != null && leftLink != null && rightLink != null;

            List<MeshRenderer> meshRenderers = new List<MeshRenderer> {};

            meshRenderers.AddRange(leftArrow.GetComponentsInChildren<MeshRenderer>(true));
            meshRenderers.AddRange(rightArrow.GetComponentsInChildren<MeshRenderer>(true));
            meshRenderers.AddRange(leftDot.GetComponentsInChildren<MeshRenderer>(true));
            meshRenderers.AddRange(rightDot.GetComponentsInChildren<MeshRenderer>(true));

            if (bomb != null)
                meshRenderers.AddRange(bomb.GetComponentsInChildren<MeshRenderer>(true));
            if (leftHead != null)
                meshRenderers.AddRange(leftHead.GetComponentsInChildren<MeshRenderer>(true));
            if (rightHead != null)
                meshRenderers.AddRange(rightHead.GetComponentsInChildren<MeshRenderer>(true));
            if (leftLink != null)
                meshRenderers.AddRange(leftLink.GetComponentsInChildren<MeshRenderer>(true));
            if (rightLink != null)
                meshRenderers.AddRange(rightLink.GetComponentsInChildren<MeshRenderer>(true));

            config.isMirrorable = true;
            foreach (var renderer in meshRenderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
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
            return leftArrow != null && rightArrow != null && leftDot != null && rightDot != null;
        }
    }
    
    [System.Serializable]
    public class Config : Qosmetics.Core.Config
    {
        public bool hasDebris = false;
        public bool hasChainHeadDebris = false;
        public bool hasChainLinkDebris = false;
        public bool hasSlider = false;
        public bool hasBomb = false;
        public bool showArrows = true;
        public bool isMirrorable = true;
    }
}