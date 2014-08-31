using System.Collections;
using System.Linq;
using ParticlePlayground;
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
        public PlaygroundParticlesC[] Particles;
        public float finishDelay;

        public void Awake ()
        {
            CurrentTries = Tries;
            Debug.Log("current tries: " + CurrentTries);
        }

        public void SuccessHit()
        {
            Debug.Log("Player Found something! O_O");

            if (Hits == Spots.Count())
            {
                GameObject.Find("KeepBetweenScenes").GetComponent<Level>().Current++;
                Promises.Promise.WithCoroutine<object>(FinishDelay(finishDelay, "winscreen"));
            }
        }

        public static IEnumerator FinishDelay(float delay, string scene)
        {
            yield return new WaitForSeconds(delay);
            Application.LoadLevel(scene);
        }

        public void FailHit()
        {
            // already won
            if (Hits == Spots.Count())
                return;

            --CurrentTries;

            Camera.main.GetComponent<CameraShake>().DoShake();

            if (CurrentTries == 0)
            {
                Promises.Promise.WithCoroutine<object>(FinishDelay(finishDelay, "losingscreen"));
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
                // star explosion
                var shockwave = Instantiate(ShockWave) as GameObject;
                var hitpos = hit.point;
                hitpos.z = -2;
                shockwave.transform.position = hitpos;

                // emitter
                var pos = hit.point;
                pos.z = -2;
                Particles[Hits].transform.position = pos;

                Particles[Hits].transform.gameObject.SetActive(true);

                // highlight star color
                Stars[Hits].color = new Color(1, 1, 1, 1);
                ++Hits;

                SuccessHit();
            }
            else
            {
                FailHit();
            }
        }
    }
}
