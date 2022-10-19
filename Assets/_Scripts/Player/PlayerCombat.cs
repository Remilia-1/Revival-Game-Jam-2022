using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerCombat : CombatUnit
{
    [Header("General References")]
    [SerializeField] private PlayerMovement playerMovementScript;
    [Space]
    [SerializeField] private Transform attacksForwardSource;
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private Animator playerAnimator;

    [Header("Melee Attack")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private FlatConeCollider meleeAttackCollider;
    [Space]
    [SerializeField] private uint meleeDamage;
    [SerializeField] private int meleeCDMsec;
    [SerializeField] private int meleeAnimDmgBufferMsec;
    private bool meleeWeaponInHand = true;
    private bool meleeOnCD = false;
    private bool meleeSecondAttackReady = false;

    [Header("Throw Attack")]
    [SerializeField] private LayerMask specialWallLayer;
    [SerializeField] private float throwLength;
    [Space]
    [SerializeField] private float throwDashSpeed;
    [SerializeField] private uint rangedDamage;
    [SerializeField] private int throwRecastCDMsec;
    [SerializeField] private Transform swordTransform;
    [SerializeField] private AnimationCurve throwCurve;

    private Transform swordParent;
    private Transform swordTarget;
    private Vector3 swordBindPointOffset;
    private Vector3 swordOriginPosition;
    private Quaternion swordOriginRotation;
    private bool canDashToSword = false;
    Vector3 swordImpactPoint;

    [Header("Misc")]
    [SerializeField] private bool drawGizmos = false;

    [System.Serializable]
    private struct FlatConeCollider
    {
        public float angle;
        public int rayCount;
        [Space]
        public float rayLength;
        public float rayHeightOffset;
    }

    // Inputs
    MainInputProfile inputs;

    private void Start()
    {
        inputs = new MainInputProfile();
        inputs.Enable();

        inputs.Main.AttackMelee.performed += ctx => MeleeAttack();

        inputs.Main.AttackThrow.performed += ctx => ThrowAttack();
        inputs.Main.AttackThrow.performed += ctx => ThrowRecast();

        swordOriginPosition = swordTransform.localPosition;
        swordOriginRotation = swordTransform.localRotation;
        swordParent = swordTransform.parent;
    }

    private async void MeleeAttack()
    {
        if (meleeOnCD || !meleeWeaponInHand)
            return;
        meleeOnCD = true;

        // Decide on animation
        if (!meleeSecondAttackReady)
            playerAnimator.Play("Anim_Slash", 0, 0.0f);
        else
            playerAnimator.Play("Anim_Slash_2", 0, 0.0f);

        // Rotate player
        playerMovementScript.CharacterRotationOverride(attacksForwardSource.eulerAngles.y, meleeCDMsec);

        // Attack damage
        await Task.Delay(meleeAnimDmgBufferMsec);
        MeleeApplyDamage(meleeDamage, meleeAttackCollider, enemyLayer);

        // Attack couldown
        await Task.Delay(meleeCDMsec - meleeAnimDmgBufferMsec);
        meleeSecondAttackReady = !meleeSecondAttackReady;
        meleeOnCD = false;
    }

    private void MeleeApplyDamage(uint damage, FlatConeCollider collider, LayerMask enemyLayer)
    {
        Vector3 heightOffset = Vector3.up * collider.rayHeightOffset;

        Vector3 rayStartPos = attackOrigin.position;
        Vector3 target = attacksForwardSource.forward * collider.rayLength;

        float angleIncrements = collider.angle / collider.rayCount;
        float startAngle = -(collider.angle / 2) + (angleIncrements / 2);

        // List of unit already hit by that cast
        List<CombatUnit> unitsHit = new List<CombatUnit> { };

        for (int i = 0; i < collider.rayCount; i++)
            if (Physics.Linecast(rayStartPos + heightOffset, rayStartPos + Quaternion.Euler(0, startAngle + angleIncrements * i, 0) * (target + heightOffset), out RaycastHit hitInfo, enemyLayer))
                if (hitInfo.collider.TryGetComponent(out CombatUnit combatUnit))
                {
                    bool unitAlreadyHit = false;

                    // Check if the unit was already hit by another ray
                    foreach (CombatUnit unit in unitsHit)
                        if (unit == combatUnit)
                            unitAlreadyHit = true;

                    // Only damage if it wasnt already hit
                    if (!unitAlreadyHit)
                    {
                        unitsHit.Add(combatUnit);
                        combatUnit.Damage(damage);
                    }
                }
    }

    private async void ThrowAttack()
    {
        if (!meleeWeaponInHand)
            return;

        Vector3 heightOffset = Vector3.up * meleeAttackCollider.rayHeightOffset;
        Vector3 rayStartPos = attackOrigin.position;

        if (!Physics.Linecast(rayStartPos + heightOffset, rayStartPos + attacksForwardSource.forward * throwLength + heightOffset, out RaycastHit hit, specialWallLayer))
            return;

        // Save impact location
        swordImpactPoint = hit.point;

        if (hit.collider != null)
        {
            swordTarget = hit.collider.transform;
            var hitDirection = (hit.point - swordTransform.position).normalized;
            var hitPoint = swordTarget.InverseTransformPoint(hit.point - hitDirection * 0.55f);
            var hitRotation = Quaternion.LookRotation(hitDirection, Quaternion.Euler(0, 90, 0) * hitDirection);

            swordTransform.rotation = hitRotation;

            // temporary solution
            StartCoroutine(ThrowOrReturnSword(swordTransform, swordTarget, hitPoint, 0.15f));
        }

        // Disable melee attack
        meleeWeaponInHand = false;

        await Task.Delay(throwRecastCDMsec);

        canDashToSword = true;
    }

    private void ThrowRecast()
    {
        if (meleeWeaponInHand || !canDashToSword)
            return;

        // Move the player
        playerMovementScript.MoveCharacterToPosition(ThrowFinish, swordImpactPoint, throwDashSpeed);
    }

    private void ThrowFinish()
    {
        swordTransform.SetParent(swordParent);
        swordTransform.localPosition = swordOriginPosition;
        swordTransform.localRotation = swordOriginRotation;

        if (swordTarget != null && swordTarget.TryGetComponent(out CombatUnit enemy))
        {
            enemy.Damage(rangedDamage);
        }

        swordTarget = null;

        meleeWeaponInHand = true;
        canDashToSword = false;
    }

    private IEnumerator ThrowOrReturnSword(Transform swordTransform, Transform target, Vector3 offset, float duration)
    {
        float timer = 0f;

        Vector3 originPosition = swordTransform.position;

        swordTransform.SetParent(target);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            swordTransform.position = Vector3.Lerp(originPosition, target.position + offset, throwCurve.Evaluate(timer / duration));

            yield return null;
        }

        if (target.TryGetComponent(out CombatUnit enemy))
        {
            enemy.Damage(rangedDamage);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.red;

        Vector3 heightOffset = Vector3.up * meleeAttackCollider.rayHeightOffset;

        Vector3 rayStartPos = attackOrigin.position;
        Vector3 target = attacksForwardSource.forward * meleeAttackCollider.rayLength;

        float angleIncrements = meleeAttackCollider.angle / meleeAttackCollider.rayCount;
        float startAngle = -(meleeAttackCollider.angle / 2) + (angleIncrements / 2);

        for (int i = 0; i < meleeAttackCollider.rayCount; i++)
        {
            Gizmos.DrawLine(rayStartPos + heightOffset, rayStartPos + Quaternion.Euler(0, startAngle + angleIncrements * i, 0) * (target + heightOffset));
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(rayStartPos + heightOffset, rayStartPos + attacksForwardSource.forward * throwLength + heightOffset);
    }
}
