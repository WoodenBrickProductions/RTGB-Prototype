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
    Disabled = 3
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

public class UnitController : TileObject
{
    [SerializeField] protected UnitStatus UnitStatus;
    [SerializeField] protected UnitStats UnitStats;

    [SerializeField] protected float AttackCooldown; // TODO: Change to attack speed?
    [SerializeField] protected float _attackTime;

    [SerializeField] protected float movementSpeed = 1;
    [SerializeField] protected int _currentState = 0;
    protected float worldMoveStep = 1;
    protected Tile _targetTile;

    protected override void Start()
    {
        base.Start();
        UnitStats.currentHealth = UnitStats.maxHealth;
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
        return UnitStats.currentHealth;
    }

    private void SetHealth(float healthChange)
    {
        if (UnitStats.currentHealth <= healthChange)
        {
            UnitStats.currentHealth = 0;
            OnHealthReachesZero();
        }
        else if (UnitStats.currentHealth + healthChange > UnitStats.currentHealth)
        {
            UnitStats.currentHealth = UnitStats.maxHealth;
        }
        else
        {
            UnitStats.currentHealth += healthChange;
        }
    }

    // Called when health reaches 0 while calling SetHealth
    protected virtual void OnHealthReachesZero()
    {
        _occupiedTile.ClearTileObject();
        ChangeState(States.Disabled);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    
    public bool GetAttacked(float damage)
    {
        if (UnitStatus.CanBeAttacked())
        {
            SetHealth(-damage);
            return true;
        }
        return false;
    }
    
    public UnitStatus GetUnitType()
    {
        return UnitStatus;
    }

    protected virtual void ChangeState(States newState)
    {
        _currentState = (int) newState;
    }
}


