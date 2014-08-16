using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Sources
{
    public class InputHandler : MonoBehaviour
    {
        public UITexture Original;
        public UITexture Fake;
        public Camera camera;
        public GameObject ShockWave;
        public Vector3 Position;
        public TextAsset Levels;
        public ArrayList Images;
        public float ToleranceRadius;
        public ImageData CurrentLevel;
        public int CurrentLevelIndex;
        public Player Player;
        public GameObject SpotSprite;
        public UILabel SpotsLeftLabel;

        public void Awake()
        {
            ParseJson();
            SetLevel(CurrentLevelIndex);
        }

        private void ParseJson()
        {
            Images = new ArrayList();
            var levels = JObject.Parse(Levels.ToString());

            foreach (JObject images in levels["images"])
            {
                var image = new ImageData { Url = images["url"].ToString() };
                image.SetDifferences(images["differences"].ToObject<List<JArray>>());
                image.SetDimension(images["dimension"].ToObject<JArray>());
                Images.Add(image);
            }
        }

        private void SetLevel(int index)
        {
            CurrentLevel = (ImageData) Images[index];
            
            var tex = Resources.Load<Texture2D>("images/" + CurrentLevel.Url);

            Original.mainTexture = tex;
            Original.SetDimensions((int)CurrentLevel.Dimension.x, (int)CurrentLevel.Dimension.y);

            tex = Resources.Load<Texture2D>("images/" + CurrentLevel.Url + "2");
            Fake.mainTexture = tex;
            Fake.SetDimensions((int)CurrentLevel.Dimension.x, (int)CurrentLevel.Dimension.y);
        }

        public void Start()
        {
//            UIEventListener.Get(Original.gameObject).onClick += (go) => GetUVHit(Original);
            UIEventListener.Get(Fake.gameObject).onClick += (go) => GetUVHit(Fake);
        }

        public Vector3 GetUVHit(UITexture go)
        {
            RaycastHit hit;
            if (!Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
                return new Vector3(-1, -1, -1);

            // get uv coordinates for user action (local world space)
            var pixelUV = go.transform.InverseTransformPoint(hit.point);
            pixelUV.x += go.width/2f;
            pixelUV.y += go.height/2f;

            // if actually hit do something fancy
            if (CurrentLevel.HasHit(pixelUV, ToleranceRadius))
            {
                // world space
                var shockwave = Instantiate(ShockWave) as GameObject;
                shockwave.transform.position = hit.point;
                
                Player.SuccessHit();

                var spotSprite = Instantiate(SpotSprite) as GameObject;
                spotSprite.transform.position = hit.point;

                if (CurrentLevel.HasSpotsLeft() <= 0)
                    Application.LoadLevel("winscreen");
            }
            else
                Player.FailHit();

            return pixelUV;
        }

        public void FixedUpdate()
        {
            SpotsLeftLabel.text = "" + CurrentLevel.HasSpotsLeft();
        }
    }
}