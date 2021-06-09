using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Serialization;

[System.Serializable]
public class UnitStats
{
    public int maxHealth;
    public int currentHealth;

    public int attackDamage;
    public int experience;
    public int level; 
    public int attackRange;
    public float attackSpeed;

    public UnitStats()
    {
        maxHealth = 1;
        currentHealth = 1;
        attackDamage = 1;
        experience = 1;
        level = 1;
        attackRange = 1;
        attackSpeed = 1;
    }
    
    public UnitStats(UnitStats unitStats)
    {
        maxHealth = unitStats.maxHealth;
        currentHealth = unitStats.currentHealth;
        attackDamage = unitStats.attackDamage;
        experience = unitStats.experience;
        level = unitStats.level;
        attackRange = unitStats.attackRange;
        attackSpeed = unitStats.attackSpeed;
    }
}
