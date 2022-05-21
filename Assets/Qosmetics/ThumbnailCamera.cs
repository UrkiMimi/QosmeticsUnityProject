using UnityEngine;


namespace Qosmetics.Core
{
    [RequireComponent(typeof(Camera))]
    public class ThumbnailCamera : MonoBehaviour
    {
        public int width = 128, height = 128;
        static private ThumbnailCamera instance = null;
        static public ThumbnailCamera Instance
        {
            get
            {
                if (instance) return instance;
                instance = FindObjectOfType<ThumbnailCamera>();
                if (instance) return instance;
                var go = new GameObject("ThumbnailCamera");
                var cam = go.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0, 0.1f, 0.2f, 1);
                instance = go.AddComponent<ThumbnailCamera>();

#if UNITY_EDITOR
                cam.transform.position = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
                cam.transform.rotation = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
#endif
                return instance;
            }
        }

        Camera cam;

        public Texture2D Generate()
        {
            cam = GetComponent<Camera>();
            if (!cam)
            {
                return Texture2D.blackTexture;
            }

            var rt = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32);
            var camPrev = cam.targetTexture;
            var rtPrev = RenderTexture.active;

            RenderTexture.active = rt;
            cam.targetTexture = rt;
            cam.Render();

            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            cam.targetTexture = camPrev;
            RenderTexture.active = rtPrev;
            RenderTexture.ReleaseTemporary(rt);

            return tex;
        }
    }
}