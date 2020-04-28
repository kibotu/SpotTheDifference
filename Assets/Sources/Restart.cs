using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Sources
{
    public class Restart : MonoBehaviour
    {
        public void OnClick()
        {
            Debug.Log("onclick");
            SceneManager.LoadScene("Scenes/game");
        }
    }
}