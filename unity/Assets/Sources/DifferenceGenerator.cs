using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Sources
{
    public class DifferenceGenerator : MonoBehaviour
    {
        public Texture2D Original;
        public Texture2D Fake;

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
                        texture2D.SetPixel(x, y, Color.red);
                    }
                }
                Debug.Log(frame);
            }
        }
    }
}