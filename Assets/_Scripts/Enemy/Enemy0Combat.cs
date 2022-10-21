using System.Threading.Tasks;
using UnityEngine;

public class Enemy0Combat : CombatUnit
{
    [Header("General References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private int damageFlashMsec;

    private int attackId = Animator.StringToHash("IsAttacking");



    public override async void OnDamaged()
    {
        Material oldMaterial = meshRenderer.material;
        meshRenderer.material = damageMaterial;

        await Task.Delay(damageFlashMsec);

        if (meshRenderer == null)
            return;
        meshRenderer.material = oldMaterial;
    }

    public void StartAttacking()
    {
        animator.SetBool(attackId, true);
    }

    public void StopAttacking()
    {
        animator.SetBool(attackId, false);
    }
}
