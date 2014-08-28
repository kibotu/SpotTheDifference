using UnityEngine;

namespace Assets.Sources
{
    public class HitTest : MonoBehaviour
    {
        public Player Player;

        public void Awake()
        {
            Player = GetComponent<Player>();
        }

        public void Update()
        {
            // mouse
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Player.Hit(hit);
                }
                else
                {
                    Player.FailHit();
                }
            }

            // touch
            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(i).position), out hit))
                {
                    Player.Hit(hit);
                }
                else
                {
                    Player.FailHit();
                }
            }
        }
    }
}
