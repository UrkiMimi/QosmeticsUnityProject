using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qosmetics.Core;

namespace Qosmetics.Notes
{
    public class Cyoob : MonoBehaviour, IExportable
    {
        private PackageInfo packageJson = default(PackageInfo);
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
        private GameObject leftSlinky;
        [SerializeField]
        private GameObject rightSlinky;

        public bool ValidateObject()
        {
            leftArrow = transform.Find("Notes/LeftArrow")?.gameObject;
            rightArrow = transform.Find("Notes/RightArrow")?.gameObject;
            leftDot= transform.Find("Notes/LeftDot")?.gameObject;
            rightDot = transform.Find("Notes/RightDot")?.gameObject;

            bomb = transform.Find("Bomb")?.gameObject;

            leftDebris = transform.Find("Debris/LeftDebris")?.gameObject;
            rightDebris = transform.Find("Debris/RightDebris")?.gameObject;

            leftSlinky = transform.Find("Sliders/LeftSlider")?.gameObject;
            rightSlinky = transform.Find("Sliders/RightSlider")?.gameObject;

            config.hasBomb = bomb != null;
            config.hasDebris = leftDebris != null && rightDebris != null;
            config.hasSlider = leftSlinky != null && rightSlinky != null;

            return leftArrow != null && rightArrow != null && leftDot != null && rightDot != null;
        }
    }
    
    [System.Serializable]
    public class Config : Qosmetics.Core.Config
    {
        public bool hasDebris = false;
        public bool hasSlider = false;
        public bool hasBomb = false;
        public bool showArrows = true;
    }
}