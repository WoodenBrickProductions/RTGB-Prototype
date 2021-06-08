﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BasicEnemyController : UnitController
{
    [SerializeField] private float wanderCooldown;
    [SerializeField] private float _wanderTime;
    
    [SerializeField] private float _moveTime;

    [SerializeField] private bool _usePseudoRandom = false;

    [SerializeField] private int agroRange = 1;
    
    private PlayerController _playerController;
    private PseudoRandomNumberGenerator _pseudoRandomNumberGenerator;
    private bool _stoppedMoving;
    private List<Position> _possibleMoves;
    private Position _lastPlayerPosition;
    
    private bool _chasing = false;
    
    // Start is called before the first frame update

    protected override void Start()
    {
        tag = "Enemy";
        _pseudoRandomNumberGenerator = new PseudoRandomNumberGenerator(4);
        base.Start();
        _attackTime = AttackCooldown;
        _possibleMoves = new List<Position>();
        _possibleMoves.Add(Position.Up);
        _possibleMoves.Add(Position.Right);
        _possibleMoves.Add(Position.Down);
        _possibleMoves.Add(Position.Left);
        _playerController = PlayerController._playerController;
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
            case 3: // DISABLED
                break;
            case 4:
                ChasingUpdate();
                break;
        }

        if (_attackTime > 0)
        {
            _attackTime -= Time.deltaTime;
        }
    }

    private void IdleUpdate()
    {
        // TODO: Can it be chasing and still end up here?
        if (!_chasing && _wanderTime <= 0)
        {
            if (Wander())
            {
                ChangeState(States.Moving);
            }
            _wanderTime = wanderCooldown;
        }
        else
        {
            float time = Time.deltaTime;
            if (_wanderTime > 0)
            {
                _wanderTime -= time;
            }
            if (_moveTime > 0)
            {
                _moveTime -= time;
            }
        }
    }

    private void MovingUpdate()
    {
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
        }else if(_moveTime <= 0)
        {
            ChangeState(_chasing ? States.Chasing : States.Idle);
        }
                    

                    
        _moveTime -= Time.deltaTime;
    }

    private void AttackingUpdate()
    {
        if (_attackTime <= 0)
        {
            TileObject occupiedTileObject = _targetTile.GetOccupiedTileObject();
            if (occupiedTileObject != null && occupiedTileObject.CompareTag("Player"))
            {
                UnitController unitController = (UnitController) occupiedTileObject;
                if (unitController.GetUnitStatus().CanBeAttacked())
                {
                    if (Attack(unitController))
                    {
                        _attackTime = AttackCooldown;
                    }
                    else
                    {
                        //Enemy unit killed
                        ChangeState(States.Idle);
                    }
                }
                else
                {
                    ChangeState(States.Chasing);
                }

            }
            else
            {
                ChangeState(States.Chasing);
            }
        }
        else
        {
            _attackTime -= Time.deltaTime;
        }
    }

    private void ChasingUpdate()
    {
        if (_chasing)
        {
            if (IsPlayerInAttackRange())
            {
                ChangeState(States.Attacking);
            }
            else
            {
                Chase();
            }
        }
        else
        {
            ChangeState(States.Idle);
        }
    }

    private void Chase()
    {
        if (FindPathToPlayer())
        {
            ChangeState(States.Moving);
        }
        else
        {
            _chasing = false;
            ChangeState(States.Idle);
        }
    }
    
    public bool IsChasing()
    {
        return _chasing;
    }

    private bool Wander()
    {
        int randomValue;
        if (_usePseudoRandom)
        {
            randomValue = _pseudoRandomNumberGenerator.GetPseudoRandomNumber();
            print("Pseudo randomed: " + randomValue);
        }
        else
        {
            randomValue = (int) (Random.value * 4);
            print("Normal randomed: " + randomValue);
        }
        
        int direction = randomValue;
        for(int i = 0; i < 4; i++)
        {
            _targetTile = boardController.GetTile(
                _position + _possibleMoves[(direction + i) % 4]);
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
                        if (occupiedTileObject.CompareTag("Player"))
                        {
                            ChangeState(States.Chasing);
                            return false;
                        }
                    }
                }
                else
                {
                    if (_targetTile.SetTileObject(this))
                    {
                        ChangeState(States.Moving);
                        _moveTime = 1.0f / movementSpeed;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    protected override void ChangeState(States newState)
    {
        switch ((int) newState)
        {
            case 0:
            {
                if (CheckForPlayerInRange())
                {
                    _lastPlayerPosition = GetPlayerPosition();
                    _chasing = true;
                    _currentState = (int) States.Chasing;
                    return;
                }
                break;
            }
            case 1:
            {
                _stoppedMoving = false;
            }
                break;
            case 2:
            {
                
            }
                break;
            case 3:
            {
                _lastPlayerPosition = GetPlayerPosition();
                _chasing = true;
            }
                break;
        }

        _currentState = (int) newState;
    }

    private Position GetPlayerPosition()
    {
        return _playerController.GetOccupiedTile().GetPosition();
    }

    private bool IsPlayerInAttackRange()
    {
        return (Position.Distance(_lastPlayerPosition, _position) <= unitStats.attackRange);
    }
    
    private bool CheckForPlayerInRange()
    {
        _lastPlayerPosition = _playerController.GetOccupiedTile().GetPosition();
        int distance = Position.Distance(_lastPlayerPosition, _position);
        if(distance <= agroRange)
        {
            return true;
        }

        return false;
    }

    private bool FindPathToPlayer()
    {
        Node start = new Node();
        start.position = _position;
        List<Position> result = new List<Position>();
        List<Position> visited = new List<Position>();
        Queue<Node> work = new Queue<Node>();
        
         
        start.history = new List<Position>();
        visited.Add(start.position);
        work.Enqueue(start);
        
        while(work.Count > 0)
        {
            Node current = work.Dequeue();
            Tile tile = boardController.GetTile(current.position);
            if (tile != null)
            {
                
                TileObject occupiedObject = tile.GetOccupiedTileObject();
                if(occupiedObject != null && occupiedObject.CompareTag("Player"))
                {
                    //Found Node
                    result = current.history;
                    result.Add(current.position);
                    _targetTile = boardController.GetTile(result[1]);
                    return true;
                }
                else
                {
                    //Didn't find Node
                    for(int i = 0; i < _possibleMoves.Count; i++)
                    {
                        Tile neighborTile = boardController.GetTile(_possibleMoves[i] + current.position);
                        if (neighborTile != null && !neighborTile.IsStaticTile() &&
                            (occupiedObject == null || occupiedObject.CompareTag("Enemy")))
                        {
                            Node currentNeighbor = new Node();
                            currentNeighbor.position = neighborTile.GetPosition();
                            if (!visited.Contains(currentNeighbor.position))
                            {
                                currentNeighbor.history = new List<Position>(current.history);
                                currentNeighbor.history.Add(current.position);
                                visited.Add(currentNeighbor.position);
                                work.Enqueue(currentNeighbor);
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private class Node
    {
        public Position position;
        public List<Position> history;
        
    }
}
