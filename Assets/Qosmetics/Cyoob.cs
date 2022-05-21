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

        public bool ShowArrows { get => config.showArrows; set => config.showArrows = value; }
        [SerializeField]
        public GameObject leftArrow;
        [SerializeField]
        public GameObject rightArrow;
        [SerializeField]
        public GameObject leftDot;
        [SerializeField]
        public GameObject rightDot;
        [SerializeField]
        public GameObject bomb;
        [SerializeField]
        public GameObject leftDebris;
        [SerializeField]
        public GameObject rightDebris;
        [SerializeField]
        public GameObject leftChainHeadDebris;
        [SerializeField]
        public GameObject rightChainHeadDebris;
        [SerializeField]
        public GameObject leftChainLinkDebris;
        [SerializeField]
        public GameObject rightChainLinkDebris;
        [SerializeField]
        public GameObject leftHead;
        [SerializeField]
        public GameObject rightHead;
        [SerializeField]
        public GameObject leftLink;
        [SerializeField]
        public GameObject rightLink;

        [SerializeField]
        public Texture2D thumbnail;

        public string ValidateObject()
        {
            leftArrow = transform.Find("Notes/LeftArrow")?.gameObject;
            rightArrow = transform.Find("Notes/RightArrow")?.gameObject;
            leftDot = transform.Find("Notes/LeftDot")?.gameObject;
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
            rightLink = transform.Find("Chains/RightLink")?.gameObject;

            config.hasBomb = bomb != null;
            config.hasDebris = leftDebris != null && rightDebris != null;

            config.hasChainHeadDebris = leftChainHeadDebris != null && rightChainHeadDebris != null;
            config.hasChainLinkDebris = leftChainLinkDebris != null && rightChainLinkDebris != null;
            config.hasSlider = leftHead != null && rightHead != null && leftLink != null && rightLink != null;

            List<MeshRenderer> meshRenderers = new List<MeshRenderer> { };

            if (leftArrow != null)
                meshRenderers.AddRange(leftArrow.GetComponentsInChildren<MeshRenderer>(true));
            if (rightArrow != null)
                meshRenderers.AddRange(rightArrow.GetComponentsInChildren<MeshRenderer>(true));
            if (leftDot != null)
                meshRenderers.AddRange(leftDot.GetComponentsInChildren<MeshRenderer>(true));
            if (rightDot != null)
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
            if (leftArrow == null)
                return "LeftArrow does not exist within the Notes object!";
            if (rightArrow == null)
                return "RightArrow does not exist within the Notes object!";
            if (leftDot == null)
                return "LeftDot does not exist within the Notes object!";
            if (rightDot == null)
                return "RightDot does not exist within the Notes object!";
            return "";
        }

        public void OnExport()
        {
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