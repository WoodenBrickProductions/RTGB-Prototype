using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats
{
    public int maxHealth = 1;
    public int currentHealth;

    public int attackDamage;
    public int experience;
    public int level;
    public int attackRange = 1;

    public UnitStats(UnitStats unitStats)
    {
        this.maxHealth = unitStats.maxHealth;
        this.currentHealth = unitStats.currentHealth;
        this.attackDamage = unitStats.attackDamage;
        this.experience = unitStats.experience;
        this.level = unitStats.level;

    }
}
