using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using UnityEngine;
using UnityEngine.Internal;

// ADD STATES ENUM, MOVE THE BLOODY PLAYER
public class PlayerController : UnitController
{
    [SerializeField] [Range(0,1)] private float movementSnapThreshold = 0.5f;
    public static PlayerController _;

    private float _moveStepMultiplier = 1;
    private bool _stoppedMoving;
    private KeyCode _inputDirection;
    private KeyCode newInput = 0;
    private KeyCode changeInput = 0;
    
    protected void Awake()
    {
        _ = this;
    }

    protected override void Start()
    {
        base.Start();
        staticObject = false;
        _moveStepMultiplier = 1 / movementSnapThreshold;
    }

    void Update()
    {
        switch (currentState)
        {
            case 0: // IDLE
                newInput = GetInputDirection();
                IdleUpdate();
                break;
            case 1: // MOVING
                MovingUpdate();
                break;
            case 2: // ATTACKING
                newInput = GetInputDirection();
                AttackingUpdate();
                break;
            case 3: // DISABLED
                break;
        }
        if (attackTime > 0)
        {
            attackTime -= Time.deltaTime;
        }
    }

    private void IdleUpdate()
    {
        _inputDirection = newInput;
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
    }

    private void MovingUpdate()
    {
        if (moveTime >= movementSnapThreshold / movementSpeed)
        {
            MoveToTile(TargetTile, WorldMoveStep * _moveStepMultiplier);
        } else if (!_stoppedMoving)
        {
            _stoppedMoving = true;
            _occupiedTile.ClearTileObject();
            _occupiedTile = TargetTile;
            // _position = _occupiedTile.GetGridPosition();
            transform.position = TargetTile.transform.position;
        }

        if(moveTime <= 0)
        {
            ChangeState(States.Idle);
        }
        
        moveTime -= Time.deltaTime;
    }
    
    private void AttackingUpdate()
    {
        if (newInput != 0 && newInput != _inputDirection)
        {
            Position targetPosition = GetTargetPosition(newInput);
            Tile newTile = boardController.GetTile(targetPosition);
            if (newTile != null && !newTile.IsStaticTile())
            {
                TileObject occupiedTileObject = newTile.GetOccupiedTileObject();
                if (occupiedTileObject != null)
                {
                    if(!occupiedTileObject.IsStaticObject())
                    {
                        if (unitStatus.CanAttack() && occupiedTileObject.CompareTag("Enemy"))
                        {
                            UnitController unitController = (UnitController) occupiedTileObject;
                            if (unitController.GetUnitStatus().CanBeAttacked())
                            {
                                changeInput = newInput;
                            }
                        }
                    }
                }
                else
                {
                    if (newTile.SetTileObject(this))
                    {
                        TargetTile = newTile;
                        _inputDirection = newInput;
                        ChangeState(States.Moving);
                        return;
                    }
                }
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
                        if (unitController.GetUnitStats().currentHealth != 0)
                        {
                            attackTime = attackCooldown;
                        }
                        else
                        {
                            ChangeState(States.Idle);
                        }
                            
                    }
                    else
                    {
                        ChangeState(States.Idle);
                        return;
                    }
                }
                else
                {
                    ChangeState(States.Idle);
                    return;
                }
            }
            else
            {
                ChangeState(States.Idle);
                return;
            }
            
            if (changeInput != 0)
            {
                Position targetPosition = GetTargetPosition(changeInput);
                Tile newTile = boardController.GetTile(targetPosition);
                if (newTile != null && !newTile.IsStaticTile())
                {
                    occupiedTileObject = newTile.GetOccupiedTileObject();
                    if (occupiedTileObject != null)
                    {
                        if(!occupiedTileObject.IsStaticObject())
                        {
                            if (unitStatus.CanAttack() && occupiedTileObject.CompareTag("Enemy"))
                            {
                                UnitController unitController = (UnitController) occupiedTileObject;
                                if (unitController.GetUnitStatus().CanBeAttacked())
                                {
                                    _inputDirection = changeInput;
                                    TargetTile = newTile;
                                    ChangeState(States.Attacking);
                                    // Continue attack state with new target
                                }
                            }
                        }
                    }
                    else
                    {
                        // Change to moving, shouldn't occur because of attack cancelling
                        TargetTile = newTile;
                        print("This shouldn't be called!");
                        if (TargetTile.SetTileObject(this))
                        {
                            ChangeState(States.Moving);
                            return;
                        }
                    }
                }
                changeInput = 0;
            }
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
            }
            break;
            case 1:
            {
                IndicatorController.SetIndicator(_inputDirection, IndicatorState.Moving);
                moveTime = 1.0f / movementSpeed;
                _stoppedMoving = false;
            }
            break;
            case 2:
            {
                IndicatorController.SetIndicator(_inputDirection, IndicatorState.Attacking);
                attackTime = attackCooldown;
            }
            break;
        }
        currentState = (int) newState;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        if (CameraController._cameraController != null)
        {
            CameraController._cameraController.SetSpeed(movementSpeed);
        }

        _moveStepMultiplier = 1 / movementSnapThreshold;
    }
}
