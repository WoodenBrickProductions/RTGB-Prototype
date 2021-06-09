using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using UnityEngine;

// ADD STATES ENUM, MOVE THE BLOODY PLAYER
public class PlayerController : UnitController
{
    public static PlayerController _playerController;
    private bool _stoppedMoving;
    private KeyCode _inputDirection;
    
    protected void Awake()
    {
        _playerController = this;
    }

    protected override void Start()
    {
        base.Start();
        staticObject = false;
        WorldMoveStep = boardController.GetWorldTileSpacing();
    }

    void Update()
    {
        switch (currentState)
        {
            case 0: // IDLE
                IdleUpdate();
                break;
            case 1: // MOVING
                MovingUpdate();
                break;
            case 2: // ATTACKING
                AttackingUpdate();
                break;
            case 3: // DISABLED
                break;
        }
    }

    private void IdleUpdate()
    {
        _inputDirection = GetInputDirection();
        if (_inputDirection != 0)
        {
            Position targetPosition = GetTargetPosition(_inputDirection);
            TargetTile = boardController.GetTile(targetPosition);
            if (TargetTile != null && !TargetTile.IsStaticTile())
            {
                TileObject occupiedTileObject = TargetTile.GetOccupiedTileObject();
                if (occupiedTileObject != null)
                {
                    if (occupiedTileObject.IsStaticObject())
                    {
                                
                    }
                    else
                    {
                        if (attackTime <= 0 && unitStatus.CanAttack() && occupiedTileObject.CompareTag("Enemy"))
                        {
                            UnitController unitController = (UnitController) occupiedTileObject;
                            if (unitController.GetUnitStatus().CanBeAttacked())
                            {
                                ChangeState(States.Attacking);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (TargetTile.SetTileObject(this))
                    {
                        ChangeState(States.Moving);
                    }
                }
            }
        }
        if (attackTime > 0)
        {
            attackTime -= Time.deltaTime;
        }
        
    }

    private void MovingUpdate()
    {
        // TODO change to constant
        if (moveTime >= 0.5 / movementSpeed)
        {
            MoveToTile(TargetTile, WorldMoveStep * 2);
        } else if (!_stoppedMoving)
        {
            _stoppedMoving = true;
            _occupiedTile.ClearTileObject();
            _occupiedTile = TargetTile;
            _position = _occupiedTile.GetPosition();
            transform.position = TargetTile.transform.position;
        }
                    
        if(moveTime <= 0)
        {
            ChangeState(States.Idle);
        }
                    
        moveTime -= Time.deltaTime;
        
        if (attackTime > 0)
        {
            attackTime -= Time.deltaTime;
        }
    }
    
    private void AttackingUpdate()
    {
        _inputDirection = GetInputDirection();
        if (_inputDirection != 0)
        {
            ;
            if (!GetTargetPosition(_inputDirection).Equals(TargetTile.GetPosition()))
            {
                ChangeState(States.Idle);
                return;
            }
        }
        
        if (attackTime <= 0)
        {
            TileObject occupiedTileObject = TargetTile.GetOccupiedTileObject();
            if (occupiedTileObject != null && occupiedTileObject.CompareTag("Enemy"))
            {
                UnitController unitController = (UnitController) occupiedTileObject;
                if (unitController.GetUnitStatus().CanBeAttacked())
                {
                    if (Attack(unitController))
                    {
                        attackTime = attackCooldown;
                    }
                    else
                    {
                        //Enemy unit killed
                        ChangeState(States.Idle);
                    }
                }
                else
                {
                    ChangeState(States.Idle);
                }

            }
            else
            {
                ChangeState(States.Idle);
            }
        }
        else
        {
            attackTime -= Time.deltaTime;
        }
    }

    public override void OnTargetObjectKilled(IAttackable target)
    {
        base.OnTargetObjectKilled(target);
        ChangeState(States.Idle);
        GameObject targetObject = target.GetGameObject();
        if (targetObject.CompareTag("Enemy"))
        {
            unitStats.experience += targetObject.GetComponent<UnitController>().GetUnitStats().experience;
        }
    }

    public override void OnDeath(DamageSource damageSource)
    {
        _occupiedTile.ClearTileObject();
        _occupiedTile = null;
        gameObject.SetActive(false);
    }

    private Position GetTargetPosition(KeyCode keyCode)
    {
        if (keyCode == KeyCode.W)
        {
            return new Position(_position.x, _position.y + 1);
        }
        else if (keyCode == KeyCode.D)
        {
            return new Position(_position.x + 1, _position.y);
        }
        else if (keyCode == KeyCode.S)
        {
            return new Position(_position.x, _position.y - 1);
        }
        else if (keyCode == KeyCode.A)
        {
            return new Position(_position.x - 1, _position.y);
        }

        return null;
    }

    private KeyCode GetInputDirection()
    {
        bool up = Input.GetKey(KeyCode.W);
        bool right = Input.GetKey(KeyCode.D);
        bool down = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        
        if (up && !(right || down || left))
        {
            return KeyCode.W;
        }
        else if (right && !(up || down || left))
        {
            return KeyCode.D;
        }
        else if (down && !(right || up || left))
        {
            return KeyCode.S;
        }
        else if (left && !(right || down || up))
        {
            return KeyCode.A;
        }

        return 0;
    }


    
    protected override void ChangeState(States newState)
    {
        switch ((int) newState)
        {
            case 0:
            {
                IndicatorController.ClearIndicator();
                currentState = 0;
            }
            break;

            case 1:
            {
                IndicatorController.SetIndicator(_inputDirection, IndicatorState.Moving);
                moveTime = 1.0f / movementSpeed;
                _stoppedMoving = false;
                currentState = 1;
            }
            break;
            case 2:
            {
                IndicatorController.SetIndicator(_inputDirection, IndicatorState.Attacking);
                attackTime = attackCooldown;
                currentState = 2;
            }
            break;
        }
    }
}
