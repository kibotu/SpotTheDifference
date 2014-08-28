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

        public Texture2D Level;
        public Rect OriginalRect;
        public Rect FakeRect;
        public Vector3 ImageScale;
        public bool ShowDifferences;

        public void Awake()
        {
            Debug.Log(Original.width + " " + Original.height);
            Debug.Log(Level.width + " " + Level.height);

//            OriginalRect.y = Level.height - OriginalRect.y;
//            FakeRect.y = Level.height - FakeRect.y;

            // original
            var originalTex = new Texture2D((int)OriginalRect.width, (int)OriginalRect.height);
            for (var y = 0; y < originalTex.height; ++y)
            {
                for (var x = 0; x < originalTex.width; ++x)
                {
                    originalTex.SetPixel(x, y,Level.GetPixel((int)(x+OriginalRect.x),(int)(y+OriginalRect.y)));
                }
            }

            OringalReplace.rectTransform.sizeDelta = new Vector2(originalTex.width, originalTex.height);
            OringalReplace.rectTransform.localScale = ImageScale;
            originalTex.Apply();
            Original = originalTex;
            OringalReplace.sprite = Sprite.Create(originalTex, new Rect(0, 0, originalTex.width, originalTex.height), new Vector2(0.5f, 0.5f), 128);


            // fake

            var fakeTex = new Texture2D((int)FakeRect.width, (int)FakeRect.height);
            for (var y = 0; y < fakeTex.height; ++y)
            {
                for (var x = 0; x < fakeTex.width; ++x)
                {
                    fakeTex.SetPixel(x, y, Level.GetPixel((int)(x + FakeRect.x), (int)(y + FakeRect.y)));
                }
            }

            Replace.rectTransform.sizeDelta = new Vector2(fakeTex.width, fakeTex.height);
            Replace.rectTransform.localScale = ImageScale;
            fakeTex.Apply();
            Fake = fakeTex;
            Replace.sprite = Sprite.Create(fakeTex, new Rect(0, 0, fakeTex.width, fakeTex.height), new Vector2(0.5f, 0.5f), 128);


            // create spots and sprites
            var tex = new Texture2D(Original.width, Original.height);
            CreateSpots(tex);

            DrawFrames(tex);
            if(ShowDifferences)Replace.sprite = Sprite.Create(tex, Replace.sprite.rect, new Vector2(0.5f, 0.5f), 128);

            tex.Apply();
//            CreateSprites();
        }

        private void CreateSpots(Texture2D tex)
        {
            for (var y = 0; y < Original.height; ++y)
            {
                for (var x = 0; x < Original.width; ++x)
                {
                    var dif = Original.GetPixel(x, y) - Fake.GetPixel(x, y);
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

            for (var y = 0; y < Original.height; ++y)
            {
                for (var x = 0; x < Original.width; ++x)
                {
                    var dif = Original.GetPixel(x, y) - Fake.GetPixel(x, y);
                    if ((dif).Equals(TransparentColor)) continue;
                    var bounds = FloodFill.floodFill(tex, Color.blue, new Vector2(x, y));
                    if (!bounds.Equals(new Rect()))
                        SpotFrames.Add(bounds);
                }
            }
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
                image.sprite = Sprite.Create(Original, frame, new Vector2(0.5f, 0.5f), 128);
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