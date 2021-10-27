using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public enum States
{
    Paused = -1, // At the Paused state, all entities are disabled;
    Idle = 0, // Not doing any checks, needs to be interacted with to Change state
    Moving = 1, // In movement
    Attacking = 2,
    Disabled = 3,
    Chasing = 4,
    Talking = 5,
    Searching = 6, // Looking for target
    Waiting = 7, // Waiting for certain condition to be met, then will take action or change state
    Acting = 8
}

public class UnitController : TileObject, IAttackable, IDealsDamage
{
    [SerializeField] protected UnitStatus unitStatus;
    [SerializeField] protected UnitStats unitStats;

    [SerializeField] protected float movementSpeed = 1;
    
    [Header("Debuging")]
    [SerializeField] protected float moveTime; // DEBUG-ONLY SERIALIZE
    [SerializeField] protected float attackTime; // DEBUG-ONLY SERIALIZE
    [SerializeField] protected States currentState = 0; // DEBUG-ONLY SERIALIZE
    [SerializeField] protected float attackCooldown; // TODO: Change to attack speed?

    protected Queue<States> state;
    protected float WorldMoveStep = 1;
    protected float WorldScalling = 1;
    protected Tile TargetTile;
    protected UIController uiController;
    protected UnitIndicatorController IndicatorController;
    protected EventQueue _eventQueue;
    
    protected override void Start()
    {
        base.Start();
        state = new Queue<States>();
        WorldScalling = boardController.GetWorldTileSpacing();
        WorldMoveStep = movementSpeed * WorldScalling;
        uiController = UIController.uiController;
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
            worldMovementStep * Time.deltaTime);
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
        print("BaseStateIsCalled");
        currentState = newState;
    }
    
    /**
     * Called when unit's currentHealth reaches 0.
     */
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
        WorldMoveStep = WorldScalling * movementSpeed;
    }


    protected virtual void Pause(GameObject sender)
    {
        if (currentState != States.Paused)
        {
            state.Enqueue((States) currentState);
            ChangeState(States.Paused);
        }
        else
        {
            currentState = state.Dequeue();
        }
    }

    private void SubscribeToEvents()
    {
        
    }

    public bool IsObjectWithinRange(TileObject target, float range)
    {
        return range < Position.Distance(_position, target.GetOccupiedTile().GetGridPosition());
    }
}


