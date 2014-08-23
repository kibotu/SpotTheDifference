using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Sources
{
    public class DifferenceGenerator : MonoBehaviour
    {
        public Texture2D Original;
        public GameObject OriginalGameObject;
        public Texture2D Fake;
        public Material Material;

        public UITexture Replace;
        public float Tolerance = 2f;

        public ArrayList Spots = new ArrayList();
        public int AmountSpots;

        private readonly Color _empty = new Color(0, 0, 0, 0);

        public void Start()
        {
            Debug.Log(Original.width + " " + Original.height);
            var tex = new Texture2D(Original.width, Original.height);
            for (var y = 0; y < Original.height; ++y)
            {
                for (var x = 0; x < Original.width; ++x)
                {
                    var dif = Original.GetPixel(x, y) - Fake.GetPixel(x, y);
                    if (!(dif).Equals(_empty))
                    {
                        dif.a = 1;
                        tex.SetPixel(x, y, Color.red);
                    } else {
                        tex.SetPixel(x, y, Color.black); 
                    }
                }
            }

            for (var y = 0; y < Original.height; ++y)
            {
                for (var x = 0; x < Original.width; ++x)
                {
                    var dif = Original.GetPixel(x, y) - Fake.GetPixel(x, y);
                    if (!(dif).Equals(_empty))
                    {
                        var bounds = FloodFill.floodFill(tex, Color.blue, new Vector2(x, y));
                        if (!bounds.Equals(new Rect()))
                        {
                            Spots.Add(bounds);
                        }
                    }
                }
            }

            DrawFrames(tex);

            Replace.mainTexture = tex;

            tex.Apply();

            AmountSpots = Spots.Count;

//            CreateSprites();
            CreateUITexture();
        }

        private void CreateUITexture()
        {
            var atlas = GetComponent<UIAtlas>();
            atlas.spriteMaterial.mainTexture = Original;

            for (var index = 0; index < Spots.Count; index++)
            {
                var frame = (Rect) Spots[index];
//                var rect = new Rect(frame.x * 1024f/800f, frame.y * 512f/588f, frame.width * 1024f / 800f, frame.height * 512f / 588f);
//                var rect = new Rect(frame.x * 800f / 1024f, frame.y * 588f / 512f, frame.width * 800f / 1024f, frame.height * 588f / 512f);
                var rect = new Rect(frame.x, frame.y , frame.width , frame.height);
                var sprite = new UISpriteData {name = "sprite" + index};
                sprite.SetRect((int)(rect.x), (int)(Original.height - rect.y - rect.height), (int)(rect.width), (int)(rect.height));
                atlas.spriteList.Add(sprite);

                var go = new GameObject("Spot");
                var uiTex = go.AddComponent<UISprite>();
                uiTex.atlas = atlas;
                uiTex.spriteName = "sprite" + index;
                uiTex.width = sprite.width;
                uiTex.height = sprite.height;
                go.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            }
        }

        private void CreateSprites()
        {
            foreach (var corner in Replace.GetComponent<UITexture>().worldCorners)
            {
                Debug.Log("c: " + corner);
            }

            Debug.Log("dim: " + Replace.mainTexture.width + " " + Replace.mainTexture.height);

            foreach (Rect frame in Spots)
            {
                var rect = new Rect(frame.x / Replace.width, frame.y / Replace.height, Replace.height / (float)Replace.mainTexture.width, Replace.height / (float)Replace.mainTexture.height);
                Debug.Log(rect);

                var s = Sprite.Create(Original, frame, new Vector2(0.5f, 0.5f),128);
                var spot = new GameObject("Spot");
//                spot.transform.localScale = Replace.transform.TransformPoint(new Vector3(frame.width, frame.height, 0));
                spot.transform.localScale = Camera.main.ScreenToWorldPoint(new Vector3(frame.width, frame.height, 0));
//                spot.transform.position = Replace.transform.TransformPoint(new Vector3(frame.x, frame.y, 0));
                spot.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(frame.x, frame.y, 0));
                var spriteRenderer = spot.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = s;
            }
        }

        private bool IsAlreadyInFrame(Vector2 p)
        {
            return Spots.Cast<Rect>().Any(spot => spot.Contains(p));
        }

        private void DrawFrames(Texture2D texture2D)
        {
            foreach (Rect frame in Spots)
            {
                for (var y = (int)frame.yMin; y < frame.yMax; ++y)
                {
                    for (var x = (int)frame.xMin; x < frame.xMax; ++x)
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