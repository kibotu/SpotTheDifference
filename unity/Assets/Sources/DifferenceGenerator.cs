using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sources
{
    public class DifferenceGenerator : MonoBehaviour
    {
        public Texture2D Original;
        public Texture2D Fake;
        public Image OringalReplace;
        public Image Replace;
        public ArrayList SpotFrames = new ArrayList();
        public Vector2 MinSize;

        private static readonly Color TransparentColor = new Color(0, 0, 0, 0);

//        public Texture2D Level;
        public Rect OriginalRect;
        public Rect FakeRect;
        public Rect DifferenceRect;
        public Vector3 ImageScale;
        public bool ShowDifferences;
        public Sprite sprite;
        public Texture2D spriteTex;
        public Texture2D LevelTex;

        public void Awake()
        {
            var Level = GameObject.Find("KeepBetweenScenes").GetComponent<Level>();
            LevelTex = Level.Textures[Level.Current];
            Debug.Log(LevelTex.width + " " + LevelTex.height);

            OringalReplace.sprite = Sprite.Create(LevelTex, new Rect(OriginalRect.x, OriginalRect.y, OriginalRect.width, OriginalRect.height), new Vector2(0.5f, 0.5f));
            Replace.sprite = ShowDifferences
                ? Sprite.Create(LevelTex, new Rect(DifferenceRect.x, DifferenceRect.y, DifferenceRect.width, DifferenceRect.height), new Vector2(0.5f, 0.5f))
                : Sprite.Create(LevelTex, new Rect(FakeRect.x, FakeRect.y, FakeRect.width, FakeRect.height), new Vector2(0.5f, 0.5f));

            OringalReplace.transform.localScale = ImageScale;
            Replace.transform.localScale = ImageScale;

            // create spots and sprites
            Original = OringalReplace.sprite.Crop();
            Fake = Replace.sprite.Crop();

            Debug.Log("Orig: " + Original.width + " " + Original.height);
            Debug.Log("Fake: " + Fake.width + " " + Fake.height);

            CreateMoochiSpots();
            CreateSprites();
        }

        private void CreateMoochiSpots()
        {
            var d = Sprite.Create(LevelTex,
                new Rect(DifferenceRect.x, DifferenceRect.y, DifferenceRect.width, DifferenceRect.height),
                new Vector2(0.5f, 0.5f), 128);

            var dTex = d.Crop();

            Debug.Log("Fake: " + dTex.width + " " + dTex.height);

            for (var y = 0; y < dTex.height; ++y)
            {
                for (var x = 0; x < dTex.width ; ++x)
                {
                    var c = dTex.GetPixel(x, y);
                    if (c.r < 0.9f || c.g > 0.1f || c.b > 0.1f || c.a < 1f)
                        continue;
                    var bounds = FloodFill.floodFill(dTex, Color.blue, new Vector2(x, y));
                    if (!bounds.Equals(new Rect()))
                        SpotFrames.Add(bounds);
                }
            }

            Debug.Log(SpotFrames.Count);

            dTex.Apply();

            spriteTex = dTex;

            sprite = Sprite.Create(dTex,
                new Rect(0, 0, dTex.width, dTex.height),
                new Vector2(0.5f, 0.5f),128);

            if (ShowDifferences) Replace.sprite = sprite;
        }

        private void CreateSpritesFromAlmostIdenticalImages()
        {
            var tex = new Texture2D(Original.width, Original.height);
            SpotFrames = CreateSpots(Original, Fake, ref tex);

            DrawFrames(tex);
            if (ShowDifferences) Replace.sprite = Sprite.Create(tex, Replace.sprite.rect, new Vector2(0.5f, 0.5f));

            tex.Apply();
            CreateSprites();
        }

        public static ArrayList CreateSpots(Texture2D original, Texture2D fake, ref Texture2D tex)
        {
            if (original.width != fake.width || original.height != fake.height)
                throw new ArgumentException("Not identical dimensions. " + "[" + original.width + ", " + original.height + "] != " + "[" + fake.width +  ", " + fake.height + "]");

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
            player.Spots = new Image[SpotFrames.Count];

            for (var index = 0; index < SpotFrames.Count; index++)
            {
                // spot
                var frame = (Rect) SpotFrames[index];
                var image = new GameObject("Spot").AddComponent<Image>();
                image.transform.parent = Replace.transform;
                try
                {
                    if (frame == null) continue;
                    image.sprite = Sprite.Create(Original, frame, new Vector2(0.5f, 0.5f));
                }
                catch (Exception e)
                {
                    Debug.Log(index + " " + frame);
                }
                image.rectTransform.anchorMin = new Vector2(0, 0);
                image.rectTransform.anchorMax = new Vector2(0, 0);
                image.rectTransform.sizeDelta = new Vector2(frame.width, frame.height);
                image.rectTransform.localScale = new Vector3(1, 1, 1);
                image.rectTransform.localPosition = new Vector3(frame.x + frame.width/2f - Original.width/2f, frame.y + frame.height/2f + -Original.height/2f, 0);
                image.enabled = false;

                var size = image.rectTransform.sizeDelta;
                size.x = Mathf.Max(size.x, MinSize.x);
                size.y = Mathf.Max(size.x, MinSize.y);

                // hitbox
                image.gameObject.AddComponent<BoxCollider>().size = size;

                // add to spot array
                player.Spots[index] = image;
            }
        }

        private void DrawFrames(Texture2D texture2D)
        {
            foreach (Rect frame in SpotFrames)
            {
                for (var y = (int)frame.yMin; y < frame.yMax; ++y)
                {
                    for (var x = (int)frame.xMin; x < (int)frame.xMax ; ++x)
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