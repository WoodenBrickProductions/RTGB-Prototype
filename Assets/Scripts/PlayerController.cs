using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using UnityEngine;

// ADD STATES ENUM, MOVE THE BLOODY PLAYER

public class PlayerController : UnitController
{
    // private List<IState> _states;

    public static PlayerController _playerController;
    private float _moveTime;
    private bool _stoppedMoving;
    
    [SerializeField] private float AttackCooldown = 1f;
    private float _attacktime = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        staticObject = false;
        // _states = new List<IState>();
        // _states.Add(new IdleState());
        // _states.Add(new MovingState());
        worldMoveStep = boardController.GetWorldTileSpacing();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
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
        }
        

    }

    private void IdleUpdate()
    {
        KeyCode direction = GetInputDirection();
        if (direction != 0)
        {
            Position targetPosition = GetTargetPosition(direction);
            _targetTile = boardController.GetTile(targetPosition);
            if (_targetTile != null && !_targetTile.IsStaticTile())
            {
                TileObject occupiedTileObject = _targetTile.GetOccupiedTileObject();
                if (occupiedTileObject != null)
                {
                    if (occupiedTileObject.IsStaticObject())
                    {
                                
                    }
                    else
                    {
                        // TODO Implement attacking
                    }
                }
                else
                {
                    _targetTile.SetTileObject(this);
                    ChangeState(States.Moving);
                    _moveTime = 1.0f / movementSpeed;
                }
            }
        }
    }

    private void MovingUpdate()
    {
        // TODO change to constant
        if (_moveTime >= 0.5 / movementSpeed)
        {
            MoveToTile(_targetTile, worldMoveStep * 2);
        } else if (!_stoppedMoving)
        {
            _stoppedMoving = true;
            transform.position = _targetTile.transform.position;
            _occupiedTile.ClearTileObject();
            _occupiedTile = _targetTile;
            _position = _occupiedTile.GetPosition();
        }
                    
        if(_moveTime <= 0)
        {
            ChangeState(States.Idle);
        }
                    
        _moveTime -= Time.deltaTime;
    }

    private void AttackingUpdate()
    {
        
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

    private void ChangeState(States newState)
    {
        switch ((int) newState)
        {
            case 0:
            {
                _currentState = 0;
            }
            break;

            case 1:
            {
                _stoppedMoving = false;
                _currentState = 1;
            }
            break;
        }
    }
}
