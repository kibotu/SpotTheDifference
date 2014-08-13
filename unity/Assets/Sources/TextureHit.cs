using UnityEngine;

namespace Assets.Sources
{
    public class TextureHit : MonoBehaviour
    {
        public Camera Camera;

        public void Update()
        {
            if (!Input.GetMouseButton(0))
                return;
            
            RaycastHit hit;
            if (!Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit))
                return;
            Debug.Log("test3");

            var renderer = hit.collider.renderer;
            var meshCollider = hit.collider as MeshCollider;
            if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
                return;

            Debug.Log("hit " + hit.textureCoord);

            var tex = (Texture2D) renderer.material.mainTexture;
            var pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            tex.SetPixel((int) pixelUV.x, (int) pixelUV.y, Color.black);
            tex.Apply();
        }
    }
}
