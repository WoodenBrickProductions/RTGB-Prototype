using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStatus
{
    [SerializeField] private bool movable = true;
    [SerializeField] private bool attackable = true;
    [SerializeField] private bool effectable = true;
    [SerializeField] private bool interactable = true;
    [SerializeField] private bool canAttack = true;

    public UnitStatus(bool movable, bool attackable, bool effectable, bool interactable, bool canAttack)
    {
        this.movable = movable;
        this.attackable = attackable;
        this.effectable = effectable;
        this.interactable = interactable;
        this.canAttack = canAttack;
    }
    
    public bool CanBeMoved()
    {
        return movable;
    } 
    
    public bool CanBeAttacked()
    {
        return attackable;
    } 

    public bool CanBeEffected()
    {
        return effectable;
    } 

    public bool CanBeInteracted()
    {
        return interactable;
    } 

    public bool CanAttack()
    {
        return canAttack;
    }

    public static UnitStatus deadUnitStatus = new UnitStatus(true, false, true, true, false);

}
