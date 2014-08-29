using UnityEngine;

namespace Assets.Sources
{
    public class Level : MonoBehaviour
    {
        public static Level Instance;

        public Texture2D [] Textures;
        public int Current;

        public void Awake()
        {
            if (!Instance)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else
                DestroyImmediate(gameObject);
        }
    }
}
