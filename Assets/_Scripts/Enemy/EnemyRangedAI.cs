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
    [SerializeField] private float movingDuration;
    [SerializeField] private float viewRange;

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

        while (timer < movingDuration && target != null)
        {
            timer += Time.deltaTime;

            movementLogic.MoveTo(target.position);

            yield return null;
        }

        movementLogic.StopWalking();
        StartCoroutine(OnAttacking());
    }

    private IEnumerator OnAttacking()
    {
        combatLogic.StartAttacking();

        yield return new WaitForSeconds(attackDuration);

        combatLogic.StopAttacking();

        StartCoroutine(OnIdle());
    }
}