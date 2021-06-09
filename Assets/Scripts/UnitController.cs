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

[Serializable]
public struct Status
{
 public bool movable;
 public bool attackable;
 public bool effectable;
 public bool interactable;
 public bool canAttack;
}

[Serializable]
public struct Stats
{
    public float maxHealth;
    public float currentHealth;
    public float attackDamage;
}

public class UnitController : TileObject, IAttackable, IDealsDamage
{
    [SerializeField] protected UnitStatus unitStatus;
    [SerializeField] protected UnitStats unitStats;

    [SerializeField] protected float AttackCooldown; // TODO: Change to attack speed?
    [SerializeField] protected float _attackTime;

    [SerializeField] protected float movementSpeed = 1;
    [SerializeField] protected int _currentState = 0;
    protected float worldMoveStep = 1;
    protected Tile _targetTile;
    private UIController uiController;
    protected UnitIndicatorController IndicatorController;
    
    
    protected override void Start()
    {
        base.Start();
        uiController = UIController.uiController;
        unitStats.currentHealth = unitStats.maxHealth;
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

    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public float GetCurrentHealth()
    {
        return unitStats.currentHealth;
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
        uiController.SetHealth(this);
    }

    // Called when health reaches 0 while calling SetHealth
    protected virtual void OnHealthReachesZero()
    {
        unitStatus.attackable = false;
        ChangeState(States.Disabled);

    }
    
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
    
    public virtual bool Attack(IAttackable target)
    {
        return target.GetAttacked(new DamageSource(this, DamageType.Attack, -unitStats.attackDamage));
    }

    public virtual void OnTargetObjectKilled(IAttackable target)
    {
        print("I: " + name + " killed " + target);
    }

    public virtual void OnDeath(DamageSource damageSource)
    {
        uiController.OnEnemyKilled(this);
        _occupiedTile.ClearTileObject();
        Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
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

    protected virtual void ChangeState(States newState)
    {
        _currentState = (int) newState;
    }


}


