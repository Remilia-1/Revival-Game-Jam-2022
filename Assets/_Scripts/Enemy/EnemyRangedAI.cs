using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAI : MonoBehaviour
{
    private enum RangedAIState
    {
        Idle,
        Moving,
        Attacking
    }

    [Header("General")]
    [SerializeField] private RangedAIState initialState;

    [Header("Idle")]
    [SerializeField] private Enemy0RangedCombat combatLogic;
    [SerializeField] private float idleDuration;

    [Header("Moving")]
    [SerializeField] private EnemyMovement movementLogic;
    [SerializeField] private Transform model;
    [SerializeField] private float movingDuration;
    [SerializeField] private float viewRange;
    [SerializeField] private float minDistToTarget, maxDistToTarget;
    [SerializeField] private float maxAngleAroundTarget;

    [Header("Attacking")]
    [SerializeField] private float attackDuration;

    private RangedAIState currentState;

    private Transform target;



    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Start()
    {
        currentState = initialState;

        var player = FindObjectOfType<PlayerMovement>();

        if (player != null)
        {
            SetTarget(player.transform);
        }

        switch (currentState)
        {
            case RangedAIState.Idle:
                StartCoroutine(OnIdle());
                break;
            case RangedAIState.Moving:
                StartCoroutine(OnMoving());
                break;
            case RangedAIState.Attacking:
                StartCoroutine(OnAttacking());
                break;
            default:
                break;
        }
    }

    private IEnumerator OnIdle()
    {
        yield return new WaitForSeconds(idleDuration);

        while (true)
        {
            if (target == null)
            {
                yield return null;

                continue;
            }

            float distance = Vector3.Distance(target.position, transform.position);

            if (distance < viewRange)
            {
                StartCoroutine(OnMoving());
                break;
            }

            yield return null;
        }
    }

    private IEnumerator OnMoving()
    {
        float timer = 0.0f;

        var targetPosition = GetRandomPositionAroundTarget(target.position, maxAngleAroundTarget, minDistToTarget, maxDistToTarget);

        while (timer < movingDuration && target != null && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            timer += Time.deltaTime;

            movementLogic.MoveTo(targetPosition);

            yield return null;
        }

        movementLogic.StopWalking();
        model.LookAt(target, Vector3.up);
        StartCoroutine(OnAttacking());
    }

    private IEnumerator OnAttacking()
    {
        combatLogic.StartAttacking();

        yield return new WaitForSeconds(attackDuration);

        combatLogic.StopAttacking();

        StartCoroutine(OnIdle());
    }

    private Vector3 GetRandomPositionAroundTarget(Vector3 target, float maxAngle, float minDist, float maxDist)
    {
        var direction = (transform.position - target);
        direction.y = 0;
        direction.Normalize();

        var angle = UnityEngine.Random.Range(-maxAngle, maxAngle);
        var dist = UnityEngine.Random.Range(minDist, maxDist);

        return (Quaternion.Euler(0, angle, 0) * direction * dist) + target;
    }

    private void OnValidate()
    {
        minDistToTarget = Mathf.Clamp(minDistToTarget, 0, float.MaxValue);
        maxDistToTarget = Mathf.Clamp(maxDistToTarget, minDistToTarget, float.MaxValue);
    }
}