using UnityEngine;

namespace Assets.Sources
{
    public class InputHandler : MonoBehaviour
    {
        public UITexture Original;
        public UITexture Fake;
        public Camera camera;
        public GameObject ShockWave;
        public Vector3 Position;

        public void Start()
        {
            UIEventListener.Get(Original.gameObject).onClick += (go) => Debug.Log( GetUVHit(Original));
            UIEventListener.Get(Fake.gameObject).onClick += (go) => Debug.Log(GetUVHit(Fake));
        }

        public Vector3 GetUVHit(UITexture go)
        {
            RaycastHit hit;
            if (!Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
                return new Vector3(-1,-1,-1);

            var tex = go.mainTexture as Texture2D;

            var shockwave = Instantiate(ShockWave) as GameObject;
            shockwave.transform.position = hit.point;

            var pixelUV = go.transform.InverseTransformPoint(hit.point);
            pixelUV.x += go.width / 2f;
            pixelUV.y += go.height / 2f;
            return pixelUV;
        }
    }
}
