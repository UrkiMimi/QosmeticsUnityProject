using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qosmetics.Core;

namespace Qosmetics.Sabers
{
    public class Whacker : MonoBehaviour, IExportable
    {
        [SerializeField]
        private PackageInfo packageJson = new PackageInfo();
        [SerializeField]
        private Qosmetics.Sabers.Config config = new Qosmetics.Sabers.Config();
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

        [SerializeField]
        Transform leftSaber = null;

        [SerializeField]
        Transform rightSaber = null;
        public bool ValidateObject()
        {
            // TODO
            var trails = GetComponentsInChildren<Trail>(true);
            config.hasTrail = trails.Length > 0;
            
            foreach (var trail in trails)
            {
                if (!trail.ValidateTrail())
                {
                    return false;
                }
            }

            leftSaber = transform.Find("LeftSaber");
            rightSaber = transform.Find("RightSaber");

            return leftSaber != null && rightSaber != null;
        }

        public void OnExport()
        {
            int id = 0;
            foreach (var trail in GetComponentsInChildren<Trail>(true))
            {
                trail.OnExport(id);
                id++;
            }
        }
    }

    [System.Serializable]
    public class Config : Qosmetics.Core.Config
    {
        public bool hasTrail = false;
    }
}