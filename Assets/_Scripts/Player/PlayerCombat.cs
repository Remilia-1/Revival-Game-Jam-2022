using UnityEngine;

public class PlayerCombat : CombatUnit
{
    [SerializeField] private Transform attacksForwardSource;

    [Header("Melee Attack")]
    [SerializeField] private Transform meleeAttackOrigin;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private uint meleeDamage;
    [Space]
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

    private void Update()
    {
        
    }

    private void MeleeAttack()
    {
        
    }

    private void MeleeApplyDamage(uint damage, FlatConeCollider collider)
    {
        Vector3 heightOffset = Vector3.up * collider.rayHeightOffset;

        Vector3 rayStartPos = meleeAttackOrigin.position;
        Vector3 target = attacksForwardSource.forward * collider.rayLength;

        float angleIncrements = collider.angle / collider.rayCount;
        float startAngle = -(collider.angle / 2) + (angleIncrements / 2);

        for (int i = 0; i < collider.rayCount; i++)
        {
            if(Physics.Linecast(rayStartPos + heightOffset, rayStartPos + Quaternion.Euler(0, startAngle + angleIncrements * i, 0) * (target + heightOffset), out RaycastHit hitInfo, enemyLayer))
                if(hitInfo.collider.TryGetComponent(out CombatUnit combatUnit))
                    combatUnit.Damage(damage);
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
