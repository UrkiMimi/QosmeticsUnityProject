using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Qosmetics.Core;

namespace Qosmetics.Sabers
{
    public class Trail : MonoBehaviour
    {
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

            public TrailData(int trailId)
            {
                this.trailId = trailId;
                colorType = ColorType.Custom;
                trailColor = Color.white;
                multiplierColor = Color.white;
                length = 14;
                whiteStep = 0.0f;
            }
        };



        TrailData data = new TrailData(0);

        private int TrailID { get => data.trailId; set => data.trailId = value; }
        public ColorType Colortype { get => data.colorType; set => data.colorType = value; }
        public Color TrailColor { get => data.trailColor; set => data.trailColor = value; }
        public Color MultiplierColor { get => data.multiplierColor; set => data.multiplierColor = value; }
        public int Length { get => data.length; set => data.length = value; }
        public float WhiteStep { get => data.whiteStep; set => data.whiteStep = value; }

        public Material trailMaterial = null;

        public Transform topTransform = null;
        public Transform bottomTransform = null;

        public bool ValidateTrail()
        {
            return topTransform != null && bottomTransform != null && trailMaterial != null;
        }
        public void OnExport(int trailId)
        {
            TrailID = trailId;
            SerializeToTextComponent();
            var renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = trailMaterial;
            }

            DestroyImmediate(this);
        }

        public void SerializeToTextComponent()
        {
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

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
