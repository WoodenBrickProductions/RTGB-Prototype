using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStatus
{
    public bool movable = true;
    public bool attackable = true;
    public bool effectable = true;
    public bool interactable = true;
    public bool canAttack = true;

    public UnitStatus(bool movable, bool attackable, bool effectable, bool interactable, bool canAttack)
    {
        this.movable = movable;
        this.attackable = attackable;
        this.effectable = effectable;
        this.interactable = interactable;
        this.canAttack = canAttack;
    }
    
    public UnitStatus(UnitStatus unitStatus)
    {
        this.movable = unitStatus.movable;
        this.attackable = unitStatus.attackable;
        this.effectable = unitStatus.effectable;
        this.interactable = unitStatus.interactable;
        this.canAttack = unitStatus.canAttack;
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
