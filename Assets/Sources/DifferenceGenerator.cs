using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Sources
{
    public class DifferenceGenerator : MonoBehaviour
    {
        public Texture2D original;
        public Texture2D fake;
        public Image originalReplace;
        public Image replace;
        public ArrayList spotFrames = new ArrayList();
        public Vector2 minSize;

        private static readonly Color TransparentColor = new Color(0, 0, 0, 0);

//        public Texture2D Level;
        public Rect originalRect;
        public Rect fakeRect;
        public Rect differenceRect;
        public Vector3 imageScale;
        public bool showDifferences;
        public Sprite sprite;
        public Texture2D spriteTex;

        public Level level;

        /**
         * Current Level Texture.
         */
        public Texture2D levelTex;

        public void Awake()
        {
            levelTex = level.textures[level.current];
            Debug.Log(levelTex.width + " " + levelTex.height);

            originalReplace.sprite = Sprite.Create(
                texture: levelTex,
                rect: new Rect(originalRect.x, originalRect.y, originalRect.width, originalRect.height),
                pivot: new Vector2(0.5f, 0.5f));

            replace.sprite = showDifferences
                ? Sprite.Create(
                    texture: levelTex,
                    rect: new Rect(differenceRect.x, differenceRect.y, differenceRect.width, differenceRect.height),
                    pivot: new Vector2(0.5f, 0.5f))
                : Sprite.Create(
                    texture: levelTex,
                    rect: new Rect(fakeRect.x, fakeRect.y, fakeRect.width, fakeRect.height),
                    pivot: new Vector2(0.5f, 0.5f));

            originalReplace.transform.localScale = imageScale;
            replace.transform.localScale = imageScale;

            // create spots and sprites
            original = originalReplace.sprite.Crop();
            fake = replace.sprite.Crop();

            Debug.Log("Orig: " + original.width + " " + original.height);
            Debug.Log("Fake: " + fake.width + " " + fake.height);

            CreateMoochiSpots();
            CreateSprites();
        }

        private void CreateMoochiSpots()
        {
            var d = Sprite.Create(levelTex,
                new Rect(differenceRect.x, differenceRect.y, differenceRect.width, differenceRect.height),
                new Vector2(0.5f, 0.5f), 128);

            var dTex = d.Crop();

            Debug.Log("Fake: " + dTex.width + " " + dTex.height);

            for (var y = 0; y < dTex.height; ++y)
            {
                for (var x = 0; x < dTex.width; ++x)
                {
                    var c = dTex.GetPixel(x, y);
                    if (c.r < 0.9f || c.g > 0.1f || c.b > 0.1f || c.a < 1f)
                        continue;
                    var bounds = FloodFill.floodFill(dTex, Color.blue, new Vector2(x, y));
                    if (!bounds.Equals(new Rect()))
                        spotFrames.Add(bounds);
                }
            }

            Debug.Log(spotFrames.Count);

            dTex.Apply();

            spriteTex = dTex;

            sprite = Sprite.Create(dTex,
                new Rect(0, 0, dTex.width, dTex.height),
                new Vector2(0.5f, 0.5f), 128);

            if (showDifferences) replace.sprite = sprite;
        }

        private void CreateSpritesFromAlmostIdenticalImages()
        {
            var tex = new Texture2D(original.width, original.height);
            spotFrames = CreateSpots(original, fake, ref tex);

            DrawFrames(tex);
            if (showDifferences) replace.sprite = Sprite.Create(tex, replace.sprite.rect, new Vector2(0.5f, 0.5f));

            tex.Apply();
            CreateSprites();
        }

        public static ArrayList CreateSpots(Texture2D original, Texture2D fake, ref Texture2D tex)
        {
            if (original.width != fake.width || original.height != fake.height)
                throw new ArgumentException("Not identical dimensions. " + "[" + original.width + ", " +
                                            original.height + "] != " + "[" + fake.width + ", " + fake.height + "]");

            for (var y = 0; y < original.height; ++y)
            {
                for (var x = 0; x < original.width; ++x)
                {
                    var dif = original.GetPixel(x, y) - fake.GetPixel(x, y);
                    if (!(dif).Equals(TransparentColor))
                    {
                        dif.a = 1;
                        tex.SetPixel(x, y, Color.red);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.black);
                    }
                }
            }

            var spots = new ArrayList();

            for (var y = 0; y < original.height; ++y)
            {
                for (var x = 0; x < original.width; ++x)
                {
                    var dif = original.GetPixel(x, y) - fake.GetPixel(x, y);
                    if ((dif).Equals(TransparentColor)) continue;
                    var bounds = FloodFill.floodFill(tex, Color.blue, new Vector2(x, y));
                    if (!bounds.Equals(new Rect()))
                        spots.Add(bounds);
                }
            }

            return spots;
        }

        private void CreateSprites()
        {
            var player = GetComponent<Player>();
            player.Spots = new Image[spotFrames.Count];

            for (var index = 0; index < spotFrames.Count; index++)
            {
                // spot
                var frame = (Rect) spotFrames[index];
                var image = new GameObject("Spot").AddComponent<Image>();
                image.transform.SetParent(replace.transform);
                try
                {
                    image.sprite = Sprite.Create(original, frame, new Vector2(0.5f, 0.5f));
                }
                catch (Exception e)
                {
                    Debug.Log(index + " " + frame);
                }

                var rectTransform = image.rectTransform;
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(frame.width, frame.height);
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.localPosition = new Vector3(frame.x + frame.width / 2f - original.width / 2f,
                    frame.y + frame.height / 2f + -original.height / 2f, 0);
                image.enabled = false;

                var size = rectTransform.sizeDelta;
                size.x = Mathf.Max(size.x, minSize.x);
                size.y = Mathf.Max(size.x, minSize.y);

                // hitbox
                image.gameObject.AddComponent<BoxCollider>().size = size;

                // add to spot array
                player.Spots[index] = image;
            }
        }

        private void DrawFrames(Texture2D texture2D)
        {
            foreach (Rect frame in spotFrames)
            {
                for (var y = (int) frame.yMin; y < frame.yMax; ++y)
                {
                    for (var x = (int) frame.xMin; x < (int) frame.xMax; ++x)
                    {
                        var c = Color.red;
                        c.r -= texture2D.GetPixel(x, y).b;
                        texture2D.SetPixel(x, y, c);
                    }
                }

                Debug.Log(frame);
            }
        }
    }
}