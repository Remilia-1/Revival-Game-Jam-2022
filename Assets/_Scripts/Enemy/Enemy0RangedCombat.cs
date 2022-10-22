using System.Threading.Tasks;
using UnityEngine;

public class Enemy0RangedCombat : CombatUnit
{
    [Header("General References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Transform projectileShootOrigin;
    [SerializeField] private Projectile projectile;
    [SerializeField] private int damageFlashMsec;
    [SerializeField] private float projectileShootDelay;
    [SerializeField] private float shootErrorAngle;

    private int attackId = Animator.StringToHash("IsAttacking");



    public void StartAttacking()
    {
        animator.SetBool(attackId, true);

        OnAttack();
    }

    public void StopAttacking()
    {
        animator.SetBool(attackId, false);
    }

    public override async void OnDamaged()
    {
        Material oldMaterial = meshRenderer.material;
        meshRenderer.material = damageMaterial;

        await Task.Delay(damageFlashMsec);

        if (meshRenderer == null)
            return;
        meshRenderer.material = oldMaterial;
    }

    private async void OnAttack()
    {
        await Task.Delay((int)(projectileShootDelay * 1000));

        var error = Quaternion.Euler(0, UnityEngine.Random.Range(-shootErrorAngle, shootErrorAngle), 0);

        var newProjectile = Instantiate(projectile, projectileShootOrigin.position, projectileShootOrigin.rotation * error);
    }
}