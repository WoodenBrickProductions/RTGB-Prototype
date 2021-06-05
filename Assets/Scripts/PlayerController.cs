﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using UnityEngine;

// ADD STATES ENUM, MOVE THE BLOODY PLAYER

public class PlayerController : UnitController
{
    private List<IState> _states;
    private int _currentState;
    private Tile _targetTile;
    
    private float moveTime;
    private float worldMoveStep;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        staticObject = false;
        _states = new List<IState>();
        _states.Add(new IdleState());
        _states.Add(new MovingState());
        worldMoveStep = boardController.GetWorldTileSpacing();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case 0:     // IDLE
                KeyCode direction = GetInputDirection();
                if (direction != 0)
                {
                    Position targetPosition = GetTargetPosition(direction);
                    _targetTile = boardController.GetTile(targetPosition);
                    if (_targetTile != null)
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
                            _currentState = (int) States.Moving;
                            moveTime = 1 / movementSpeed;
                        }
                    }
                }
                break;
            case 1: // MOVING
                {
                    if (moveTime >= 0f)
                    {
                        MoveToTile(_targetTile, worldMoveStep);
                        moveTime -= Time.deltaTime;
                    }
                    else
                    {
                        transform.position = _targetTile.transform.position;
                        _occupiedTile.ClearTileObject();
                        _occupiedTile = _targetTile;
                        _position = _occupiedTile.GetPosition();
                        _currentState = (int) States.Idle;
                    }
                }
                break;
        }
        

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
    
    
    
    
    
    // private bool MoveToTarget(Position position)
    // {
    //     _targetTile = boardController.GetTile(position);
    //     if (_targetTile.GetOccupiedTileObject() == null)
    //     {
    //         _currentState = 
    //     }
    // }
    
    void Move(Vector3 targetLocation)
    {

    }

}