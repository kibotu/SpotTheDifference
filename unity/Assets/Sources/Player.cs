using UnityEngine;

namespace Assets.Sources
{
    public class Player : MonoBehaviour
    {
        public ProgressBar Bar;
        public int Tries;
        public int CurrentTries;

        public void Start ()
        {
            CurrentTries = Tries;
        }

        public void SuccessHit()
        {
            Debug.Log("Player Found something! O_O");
        }

        public void FailHit()
        {
            --CurrentTries;
            Bar.SetProgress(CurrentTries / (float) Tries);

            if (CurrentTries <= 0)
            {
                Application.LoadLevel("losingscreen");
                Debug.Log("Player Lost.");
            }
        }
    }
}
