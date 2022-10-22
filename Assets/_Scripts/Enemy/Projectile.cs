using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;



    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        CheckCollision();
    }

    private void CheckCollision()
    {
        // TODO: add some hit logic here
    }
}
