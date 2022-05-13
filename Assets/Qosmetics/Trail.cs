using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Qosmetics.Core;

namespace Qosmetics.Sabers
{
    [System.Serializable]
    public class Trail : MonoBehaviour
    {
        [System.Serializable]
        struct STrailColor
        {
            [SerializeField]
            float r;
            [SerializeField]
            float g;
            [SerializeField]
            float b;
            [SerializeField]
            float a;

            public static implicit operator STrailColor(Color color) => new STrailColor(color);
            public static implicit operator Color(STrailColor color) => new Color(color.r, color.g, color.b, color.a);
            public STrailColor(Color color)
            {
                r = color.r;
                g = color.g;
                b = color.b;
                a = color.a;

            }
        };

        [System.Serializable]
        struct TrailData
        {
            [SerializeField]
            public int trailId;

            [SerializeField]
            public ColorType colorType;

            [SerializeField]
            public Trail.STrailColor trailColor;

            [SerializeField]
            public Trail.STrailColor multiplierColor;

            [SerializeField]
            public int length;

            [SerializeField]
            public float whiteStep;

            public TrailData(int trailId, Trail trail)
            {
                this.trailId = trailId;
                colorType = trail.colorType;
                trailColor = trail.trailColor;
                multiplierColor = trail.multiplierColor;
                length = trail.length;
                whiteStep = trail.whiteStep;
            }
        };


        [SerializeField]
        int trailId = 0;
        [SerializeField]
        ColorType colorType = ColorType.Custom;
        [SerializeField]
        Color trailColor = Color.white;
        [SerializeField]
        Color multiplierColor = Color.white;
        [SerializeField]
        int length = 14;
        [SerializeField]
        float whiteStep = 0.0f;

        private int TrailID { get => trailId; set => trailId = value; }
        public ColorType Colortype { get => colorType; set => colorType = value; }
        public Color TrailColor { get => trailColor; set => trailColor = value; }
        public Color MultiplierColor { get => multiplierColor; set => multiplierColor = value; }
        public int Length { get => length; set => length = value; }
        public float WhiteStep { get => whiteStep; set => whiteStep = value; }

        [SerializeField]
        public Material trailMaterial = null;

        [SerializeField]
        public Transform topTransform = null;
        [SerializeField]
        public Transform bottomTransform = null;

        public string ValidateTrail()
        {
            if (topTransform == null)
                return "Top Transform was not set on a trail";
            if (bottomTransform == null)
                return "Bottom Transform was not set on a trail";
            if (trailMaterial == null)
                return "Trail Material was not set on a trail";
            return "";
        }

        public void OnExport(int trailId)
        {
            TrailID = trailId;
            SerializeToTextComponent();
            var renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer == null)
                renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = trailMaterial;

            DestroyImmediate(this);
        }

        public void SerializeToTextComponent()
        {
            string json = JsonConvert.SerializeObject(new TrailData(TrailID, this), new JsonSerializerSettings { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            gameObject.AddOrSetTextComponent(json);

            string topTransformJson = JsonConvert.SerializeObject(new SerializedTrail(true, TrailID), Formatting.Indented);
            string botTransformJson = JsonConvert.SerializeObject(new SerializedTrail(false, TrailID), Formatting.Indented);

            topTransform.gameObject.AddOrSetTextComponent(topTransformJson);
            bottomTransform.gameObject.AddOrSetTextComponent(botTransformJson);
        }

        struct SerializedTrail
        {
            [SerializeField]
            bool isTop;
            [SerializeField]
            int trailId;

            public SerializedTrail(bool isTop, int trailId)
            {
                this.isTop = isTop;
                this.trailId = trailId;
            }
        };

        public enum ColorType
        {
            Left,
            Right,
            Custom
        }
    }
}
