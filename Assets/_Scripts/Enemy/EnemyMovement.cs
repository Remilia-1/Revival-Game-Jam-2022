using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform model;
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rigidBody;



    public void MoveTo(Vector3 target)
    {
        rigidBody.position = Vector3.MoveTowards(rigidBody.position, target, moveSpeed * Time.deltaTime);

        var direction = (target - rigidBody.position).normalized;
        direction.y = 0;

        model.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
}
