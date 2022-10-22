using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform model;
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rigidBody;
    private int isWalkingId = Animator.StringToHash("IsWalking");



    public void MoveTo(Vector3 target)
    {
        animator.SetBool(isWalkingId, true);
        rigidBody.position = Vector3.MoveTowards(rigidBody.position, target, moveSpeed * Time.deltaTime);

        var direction = (target - rigidBody.position).normalized;
        direction.y = 0;

        model.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void StopWalking()
    {
        animator.SetBool(isWalkingId, false);
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
}
