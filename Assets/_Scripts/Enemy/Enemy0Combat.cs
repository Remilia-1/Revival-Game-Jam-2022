using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy0Combat : CombatUnit
{
    [Header("General References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private int damageFlashMsec;

    [Header("Attack")]
    [SerializeField] private uint damagePerSec;
    [SerializeField] private float attackRadius;

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
        StartCoroutine(DamageOverTime());
    }

    IEnumerator DamageOverTime()
    {
        var player = FindObjectOfType<PlayerMovement>();

        if ((player.transform.position - transform.position).magnitude < attackRadius)
            player.GetComponent<CombatUnit>().Damage((damagePerSec / 10));

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(DamageOverTime());
    }

    public void StopAttacking()
    {
        animator.SetBool(attackId, false);
        StopAllCoroutines();
    }
}
