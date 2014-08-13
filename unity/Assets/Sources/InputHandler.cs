using UnityEngine;

namespace Assets.Sources
{
    public class InputHandler : MonoBehaviour
    {
        public UITexture Original;
        public UITexture Fake;
        public Camera camera;
        public Vector3 Position;

        public void Start()
        {
            UIEventListener.Get(Original.gameObject).onClick += (go) =>
            {
                RaycastHit hit;
                if (!Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
                    return;
                
                var tex = Original.mainTexture as Texture2D;

                Vector3 pixelUV = Original.transform.InverseTransformPoint(hit.point);
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;

                Debug.Log(hit.textureCoord);
            };

        }
    }
}
