using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sources
{
    public class Player : MonoBehaviour
    {
        public GameObject ShockWave;
        public int Tries;
        public int CurrentTries;
        public int Hits;
        public Image[] Stars;
        public Image [] Spots;

        public void Start ()
        {
            CurrentTries = Tries;
        }

        public void SuccessHit()
        {
            Debug.Log("Player Found something! O_O");
            Stars[Hits].color = new Color(1,1,1,1);
            ++Hits;

            if (Hits >= Spots.Count())
                Application.LoadLevel("winscreen");
        }

        public void FailHit()
        {
            --CurrentTries;

            if (CurrentTries <= 0)
            {
                Application.LoadLevel("losingscreen");
                Debug.Log("Player Lost.");
            }
        }

        public void Hit(RaycastHit hit)
        {
            Debug.Log(hit.transform.gameObject.name);
            var image = hit.transform.gameObject.GetComponent<Image>();
            if (!image.enabled)
            {
                image.enabled = true;

                // world space
                var shockwave = Instantiate(ShockWave) as GameObject;
                var hitpos = hit.point;
                hitpos.z = -10;
                shockwave.transform.position = hitpos;


                SuccessHit();
            }
            else
            {
                FailHit();
            }
        }
    }
}
