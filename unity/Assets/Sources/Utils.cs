using UnityEngine;

namespace Assets.Sources
{
    public static class Utils
    {
        public static Vector2 GetPNGTextureSize(this Texture2D tex)
        {
            var dim = new Vector2();

            // check only png tex!!! // http://www.libpng.org/pub/png/spec/1.2/PNG-Structure.html
            byte[] png_signature = { 137, 80, 78, 71, 13, 10, 26, 10 };

            const int cMinDownloadedBytes = 30;

            byte[] buf = tex.EncodeToPNG();
            if (buf.Length > cMinDownloadedBytes)
            {
                // now we can check png format
                for (int i = 0; i < png_signature.Length; i++)
                {
                    if (buf[i] != png_signature[i])
                    {
                        Debug.LogWarning("Error! Texture os NOT png format!");
                        return dim; // this is NOT png file!
                    }
                }

                // now get width and height of texture
                dim.x = buf[16] << 24 | buf[17] << 16 | buf[18] << 8 | buf[19];
                dim.y = buf[20] << 24 | buf[21] << 16 | buf[22] << 8 | buf[23];

                Debug.Log("Loaded texture size: width = " + dim.x + "; height = " + dim.y);
            }

            return dim;
        }
    }
}
