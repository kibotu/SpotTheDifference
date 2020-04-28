using UnityEngine;

public class ShockwaveScript : MonoBehaviour
{
    public float speed = 0.01f;

    public float maxSize = 1f;

    private Vector3 speedVector;

    void Update()
    {
        speedVector.x = speed * Time.deltaTime;
        speedVector.y = speed * Time.deltaTime;
        speedVector.z = speed * Time.deltaTime;

        transform.localScale += speedVector;
        if (transform.localScale.x >= maxSize)
        {
            // transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            Destroy(gameObject);
        }
    }
}