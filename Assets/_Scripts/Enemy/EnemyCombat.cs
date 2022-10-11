using System.Threading.Tasks;
using UnityEngine;

public class EnemyCombat : CombatUnit
{
    [Header("General References")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private int damageFlashMsec;

    void Start()
    {
        
    }

    void Update()
    {
        
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
}
