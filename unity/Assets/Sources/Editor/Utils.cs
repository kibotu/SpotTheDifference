using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.Sources.Editor
{
    public static class Utils
    {
        public static Vector2 GetImageSize(this Texture2D asset)
        {
            var dimension = new Vector2();
            if (asset == null) return dimension;
            var assetPath = AssetDatabase.GetAssetPath(asset);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return dimension;
            var args = new object[] {0, 0};
            var mi = typeof (TextureImporter).GetMethod("GetWidthAndHeight",
                BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(importer, args);
            dimension.x = (int) args[0];
            dimension.y = (int) args[1];
            return dimension;
        }
    }
}