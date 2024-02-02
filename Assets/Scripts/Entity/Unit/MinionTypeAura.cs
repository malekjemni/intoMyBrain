using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TowerBase;

public class MinionTypeAura : MonoBehaviour
{
    public UnitBase.UnitType unitType;
    private UnitBase unit;
    private UnitAbilityHandler abilityHandler;

    private void Start()
    {
        unit = GetComponent<UnitBase>();
        abilityHandler = GetComponent<UnitAbilityHandler>();
        switch (unitType)
        {
            case UnitBase.UnitType.Friendly:
                abilityHandler.AddAbility(new FriendlyAura(), unit);
                break;
            case UnitBase.UnitType.Hostile:
                abilityHandler.AddAbility(new HostileAura(), unit);
                break;
            default:
                break;
        }
        
    }
}
