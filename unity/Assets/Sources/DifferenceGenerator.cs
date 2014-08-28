using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sources
{
    public class DifferenceGenerator : MonoBehaviour
    {
        public Texture2D Original;
        public GameObject OriginalGameObject;
        public Texture2D Fake;
        public Material Material;

        public Image Replace;
        public float Tolerance = 2f;

        public ArrayList Spots = new ArrayList();
        public int AmountSpots;

        private readonly Color _empty = new Color(0, 0, 0, 0);
        public ArrayList GameObjects = new ArrayList();

        public void Awake()
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

            Replace.sprite = Sprite.Create(Fake, Replace.sprite.rect, new Vector2(0.5f, 0.5f), 128);

            tex.Apply();

            AmountSpots = Spots.Count;
            CreateSprites();
        }

        private void CreateSprites()
        {
            foreach (Rect frame in Spots)
            {
                var image = new GameObject("Spot").AddComponent<Image>();
                image.transform.parent = Replace.transform;
                image.sprite = Sprite.Create(Original, frame, new Vector2(0.5f, 0.5f),128);
                image.rectTransform.anchorMin = new Vector2(0,0);
                image.rectTransform.anchorMax = new Vector2(0,0);
                image.rectTransform.sizeDelta = new Vector2(frame.width, frame.height);
                image.rectTransform.localScale = new Vector3(1, 1, 1);
                image.rectTransform.localPosition = new Vector3(frame.x + frame.width / 2f - Original.width / 2f, frame.y + frame.height / 2f + -Original.height / 2f, 0);
//                image.rectTransform.localPosition = new Vector3(0,0, 0);
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