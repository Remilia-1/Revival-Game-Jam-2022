using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAI : MonoBehaviour
{
    private enum MeleeAIState
    {
        Idle,
        Chasing,
        Attacking
    }

    [Header("General")]
    [SerializeField] private MeleeAIState initialState;

    [Header("Idle")]
    [SerializeField] private Enemy0Combat combatLogic;
    [SerializeField] private float idleDuration;

    [Header("Chasing")]
    [SerializeField] private EnemyMovement movementLogic;
    [SerializeField] private float chasingDuration;
    [SerializeField] private float viewRange;

    [Header("Attacking")]
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRange;

    private MeleeAIState currentState;

    private Transform target;



    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Start()
    {
        currentState = initialState;

        var player = FindObjectOfType<PlayerMovement>();

        if(player != null)
        {
            SetTarget(player.transform);
        }

        switch (currentState)
        {
            case MeleeAIState.Idle:
                StartCoroutine(OnIdle());
                break;
            case MeleeAIState.Chasing:
                StartCoroutine(OnChasing());
                break;
            case MeleeAIState.Attacking:
                StartCoroutine(OnAttacking());
                break;
            default:
                break;
        }
    }

    private IEnumerator OnIdle()
    {
        yield return new WaitForSeconds(idleDuration);

        while(true)
        {
            if (target == null)
            {
                yield return null;

                continue;
            }

            float distance = Vector3.Distance(target.position, transform.position);

            if(distance < attackRange)
            {
                StartCoroutine(OnAttacking());
                break;
            }
            else if (distance < viewRange)
            {
                StartCoroutine(OnChasing());
                break;
            }

            yield return null;
        }
    }

    private IEnumerator OnChasing()
    {
        float timer = 0.0f;

        while(timer < chasingDuration && target != null)
        {
            timer += Time.deltaTime;

            movementLogic.MoveTo(target.position);

            if(Vector3.Distance(target.position, transform.position) < attackRange)
            {
                movementLogic.StopWalking();
                StartCoroutine(OnAttacking());
                break;
            }

            yield return null;
        }

        if(timer > chasingDuration)
        {
            movementLogic.StopWalking();
            StartCoroutine(OnIdle());
        }
    }

    private IEnumerator OnAttacking()
    {
        combatLogic.StartAttacking();

        yield return new WaitForSeconds(attackDuration);

        combatLogic.StopAttacking();

        StartCoroutine(OnIdle());
    }
}
