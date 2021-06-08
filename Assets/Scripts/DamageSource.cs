using UnityEngine;

public enum DamageType
{
    Attack,
    Effect
}


public class DamageSource
{
    private IDealsDamage _damageSource;
    private DamageType _damageType;
    private int _damageAmount;
    
    public DamageSource(IDealsDamage source, DamageType type, int damage)
    {
        _damageSource = source;
        _damageType = type;
        _damageAmount = damage;
    }

    public DamageType GetDamageType()
    {
        return _damageType;
    }

    public IDealsDamage GetDamageSourceObject()
    {
        return _damageSource;
    }

    public int GetDamageAmount()
    {
        return _damageAmount;
    }
}
