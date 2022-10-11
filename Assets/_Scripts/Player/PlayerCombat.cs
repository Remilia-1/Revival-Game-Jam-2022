using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CombatUnit
{
    [SerializeField] private Transform attacksForwardSource;

    [Header("Melee Attack")]
    [SerializeField] private Transform meleeAttackOrigin;
    [Space]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private uint meleeDamage;
    [SerializeField] private FlatConeCollider meleeAttackCollider;

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
    }

    private void MeleeAttack()
    {
        MeleeApplyDamage(meleeDamage, meleeAttackCollider, enemyLayer);
    }

    private void MeleeApplyDamage(uint damage, FlatConeCollider collider, LayerMask enemyLayer)
    {
        Vector3 heightOffset = Vector3.up * collider.rayHeightOffset;

        Vector3 rayStartPos = meleeAttackOrigin.position;
        Vector3 target = attacksForwardSource.forward * collider.rayLength;

        float angleIncrements = collider.angle / collider.rayCount;
        float startAngle = -(collider.angle / 2) + (angleIncrements / 2);

        // List of unit already hit by that cast
        List<CombatUnit> unitsHit = new List<CombatUnit> { };

        for (int i = 0; i < collider.rayCount; i++)
        {
            if(Physics.Linecast(rayStartPos + heightOffset, rayStartPos + Quaternion.Euler(0, startAngle + angleIncrements * i, 0) * (target + heightOffset), out RaycastHit hitInfo, enemyLayer))
                if(hitInfo.collider.TryGetComponent(out CombatUnit combatUnit))
                {
                    bool unitAlreadyHit = false;

                    // Check if the unit was already hit by another ray
                    foreach(CombatUnit unit in unitsHit)
                        if(unit == combatUnit)
                            unitAlreadyHit = true;

                    // Only damage if it wasnt already hit
                    if (!unitAlreadyHit)
                    {
                        unitsHit.Add(combatUnit);
                        combatUnit.Damage(damage);
                    }
                }
        }
    }

    private void AttackThrow()
    {
        // Something fun;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.red;

        Vector3 heightOffset = Vector3.up * meleeAttackCollider.rayHeightOffset;

        Vector3 rayStartPos = meleeAttackOrigin.position;
        Vector3 target = attacksForwardSource.forward * meleeAttackCollider.rayLength;

        float angleIncrements = meleeAttackCollider.angle / meleeAttackCollider.rayCount; 
        float startAngle = -(meleeAttackCollider.angle / 2) + (angleIncrements / 2);

        for (int i = 0; i < meleeAttackCollider.rayCount; i++)
        {
            Gizmos.DrawLine(rayStartPos + heightOffset, rayStartPos + Quaternion.Euler(0, startAngle + angleIncrements * i, 0) * (target + heightOffset));
        }
    }
}
