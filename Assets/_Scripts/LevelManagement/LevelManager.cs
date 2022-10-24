using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelEndPlatform[] targetPlatforms;
    
    private List<CombatUnit> activeUnits;
    private bool IsAvailable => activeUnits.Count == 0;



    private void Awake() 
    {
        var units = FindObjectsOfType<CombatUnit>();

        activeUnits = new List<CombatUnit>(units.Length);

        foreach (var unit in units)
        {
            if (unit is Enemy0Combat || unit is Enemy0RangedCombat)
            {
                activeUnits.Add(unit);

                unit.onDieEvent += OnUnitDiedHandler;
            }
        }

        foreach (var platform in targetPlatforms)
        {
            platform.onLevelEndEntered += OnLevelEndEntered;
        }
    }

    private void OnUnitDiedHandler(CombatUnit unit)
    {
        activeUnits.Remove(unit);
    }

    private void OnLevelEndEntered() 
    {
        // 
    }
}