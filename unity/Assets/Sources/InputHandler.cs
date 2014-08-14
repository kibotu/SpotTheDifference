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
            UIEventListener.Get(Original.gameObject).onClick += (go) => Debug.Log(GetUVHit(Original, camera));
            UIEventListener.Get(Fake.gameObject).onClick += (go) => Debug.Log(GetUVHit(Fake, camera));
        }

        public static Vector3 GetUVHit(UITexture go, Camera camera)
        {
            RaycastHit hit;
            if (!Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
                return new Vector3(-1,-1,-1);

            var tex = go.mainTexture as Texture2D;

            var pixelUV = go.transform.InverseTransformPoint(hit.point);
            pixelUV.x += go.width / 2f;
            pixelUV.y += go.height / 2f;
            return pixelUV;
        }
    }
}
