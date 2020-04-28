using UnityEngine;

namespace Assets.Sources
{
    public class HitTest : MonoBehaviour
    {
        public Player player;

        public void Awake()
        {
            player = GetComponent<Player>();
        }

        public void Update()
        {
            // mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
                {
                    player.Hit(hit);
                }
                else
                {
                    player.FailHit();
                }
            }

            // touch
            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(i).position), out var hit))
                {
                    player.Hit(hit);
                }
                else
                {
                    player.FailHit();
                }
            }
        }
    }
}
