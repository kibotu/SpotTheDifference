using UnityEngine;

namespace Assets.Sources
{
    public static class ExtensionMethods
    {

        public static Texture2D Crop(this Sprite s)
        {
            var t = new Texture2D((int)s.rect.width, (int)s.rect.height,TextureFormat.ARGB32, false,true);
            t.SetPixels(s.texture.GetPixels((int)s.textureRect.x, (int)s.textureRect.y, (int)s.textureRect.width, (int)s.textureRect.height));
            t.Apply();
            return t;
        }
    }
}