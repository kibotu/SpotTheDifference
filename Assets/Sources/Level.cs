using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Sources
{
    public class Level : MonoBehaviour
    {
        public static Level instance;

        public Texture2D[] textures;
        public int current;

        public void Awake()
        {
            if (!instance)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else
                DestroyImmediate(gameObject);
        }
    }
}