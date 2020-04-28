using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Sources
{
    public class Restart : MonoBehaviour
    {
        public void OnClick() => SceneManager.LoadScene("Scenes/game");
    }
}