using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public enum States
{
    Idle = 0,
    Moving = 1,
    Attacking = 2,
    Disabled = 3,
    Chasing = 4
}

public class UnitController : TileObject, IAttackable, IDealsDamage
{
    [SerializeField] protected UnitStatus unitStatus;
    [SerializeField] protected UnitStats unitStats;

    [SerializeField] protected float movementSpeed = 1;
    
    [Header("Debuging")]
    [SerializeField] protected float moveTime; // DEBUG-ONLY SERIALIZE
    [SerializeField] protected float attackTime; // DEBUG-ONLY SERIALIZE
    [SerializeField] protected int currentState = 0; // DEBUG-ONLY SERIALIZE
    [SerializeField] protected float attackCooldown; // TODO: Change to attack speed?
    
    
    protected float WorldMoveStep = 1;
    protected Tile TargetTile;
    private UIController _uiController;
    protected UnitIndicatorController IndicatorController;
    
    
    protected override void Start()
    {
        base.Start();
        _uiController = UIController.uiController;
        unitStats.currentHealth = unitStats.maxHealth;
        attackCooldown = 1.0f / unitStats.attackSpeed;
        IndicatorController = GetComponentInChildren<UnitIndicatorController>();
    }

    protected void MoveToTile(Tile tile, float worldMovementStep)
    {
        // TODO is this optimal?
        transform.position = Vector3.MoveTowards(
            transform.position,
            tile.transform.position,
            movementSpeed * worldMovementStep * Time.deltaTime);
    }

    private void SetHealth(int healthChange)
    {
        if (unitStats.currentHealth + healthChange <= 0)
        {
            unitStats.currentHealth = 0;
            OnHealthReachesZero();
        }
        else if (unitStats.currentHealth + healthChange > unitStats.currentHealth)
        {
            unitStats.currentHealth = unitStats.maxHealth;
        }
        else
        {
            unitStats.currentHealth += healthChange;
        }
        _uiController.SetHealth(this);
    }

    /**
     * Called when currentHealth reaches zero, before calling OnDeath.
     */
    protected virtual void OnHealthReachesZero()
    {
        unitStatus.attackable = false;
        ChangeState(States.Disabled);

    }
    
    /**
     * Called when this unit is being attacked by a damageSource.
     */
    public virtual bool GetAttacked(DamageSource damageSource)
    {
        if (unitStatus.CanBeAttacked())
        {
            SetHealth(damageSource.GetDamageAmount());
            if (unitStats.currentHealth == 0)
            {
                damageSource.GetDamageSourceObject().OnTargetObjectKilled(this);
                OnDeath(damageSource);
            }
            return true;
        }
        return false;
    }
    
    /**
     * Called when this unit attacks another unit.
     */
    public virtual bool Attack(IAttackable target)
    {
        return target.GetAttacked(new DamageSource(this, DamageType.Attack, -unitStats.attackDamage));
    }

    /**
     * Called when this unit kills another unit.
     */
    public virtual void OnTargetObjectKilled(IAttackable target)
    {
        print("I: " + name + " killed " + target);
    }

    protected virtual void ChangeState(States newState)
    {
        currentState = (int) newState;
    }
    
    /**
     * Called when unit's currentHealth reaches 0.
     */
    public virtual void OnDeath(DamageSource damageSource)
    {
        _uiController.OnEnemyKilled(this);
        _occupiedTile.ClearTileObject();
        Destroy(gameObject);
        gameObject.SetActive(false);
    }
    
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public UnitStatus GetUnitStatus()
    {
        UnitStatus status = new UnitStatus(unitStatus);
        return status;
    }

    public UnitStats GetUnitStats()
    {
        UnitStats stats = new UnitStats(unitStats);
        return stats;
    }
    
    protected virtual void OnValidate()
    {
        attackCooldown = 1.0f / unitStats.attackSpeed;
    }
}


