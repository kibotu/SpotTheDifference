using UnityEngine;

namespace Assets.Sources
{
    public class Restart : MonoBehaviour {


        public void OnClick()
        {
            Application.LoadLevel("Game");
        }
    }
}