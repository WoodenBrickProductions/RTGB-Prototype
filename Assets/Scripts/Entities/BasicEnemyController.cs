using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class BasicEnemyController : UnitController
{
    [Header("AI stats")]
    [SerializeField] private float wanderCooldown;
    [SerializeField] private int agroRange = 1;
    [SerializeField] private int chasingRange = 100;
    [SerializeField] private float chasingCooldown = 0.25f;
    [SerializeField] private bool usePseudoRandom = false;
    
    
    [Header("Debuging")]
    [SerializeField] private float wanderTime;
    [SerializeField] private float chasingTime;
    [SerializeField] private bool chasing = false;
    
    private PlayerController _playerController;
    private PseudoRandomNumberGenerator _pseudoRandomNumberGenerator;
    
    private List<Position> _possibleMoves;
    private bool _stoppedMoving;
    private Position _lastPlayerPosition;
    
    
    // Start is called before the first frame update

    protected override void Start()
    {
        tag = "Enemy";
        wanderTime = wanderTime + Random.value;
        _pseudoRandomNumberGenerator = new PseudoRandomNumberGenerator(4);
        base.Start();
        attackTime = attackCooldown;
        _possibleMoves = new List<Position>();
        _possibleMoves.Add(Position.Up);
        _possibleMoves.Add(Position.Right);
        _possibleMoves.Add(Position.Down);
        _possibleMoves.Add(Position.Left);
        _playerController = PlayerController._;
        _playerController.OnPauseAll += Pause;
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
            case 4:
                ChasingUpdate();
                break;
        }

        if (attackTime > 0)
        {
            attackTime -= Time.deltaTime;
        }
        
        if (chasingTime > 0)
        {
            chasingTime -= Time.deltaTime;
        }
    }

    private void IdleUpdate()
    {
        if (!chasing && wanderTime <= 0)
        {
            if (_playerController.GetUnitStatus().CanBeAttacked() && IsPlayerInRange(agroRange))
            {
                ChangeState(States.Chasing);
                return;
            }
            if (Wander())
            {
                if (TargetTile.SetTileObject(this))
                {
                    ChangeState(States.Moving);
                }
            }
            wanderTime = wanderCooldown;
        }
        else
        {
            float time = Time.deltaTime;
            if (wanderTime > 0)
            {
                wanderTime -= time;
            }
            if (moveTime > 0)
            {
                moveTime -= time;
            }
        }
    }
    /**
     * Set next possible tile to wander to, set's TargetTile
     */
    private bool Wander()
    {
        int randomValue;
        if (usePseudoRandom)
        {
            randomValue = _pseudoRandomNumberGenerator.GetPseudoRandomNumber();
        }
        else
        {
            randomValue = (int) (Random.value * 4);
        }
        
        int direction = randomValue;
        for(int i = 0; i < 4; i++)
        {
            TargetTile = boardController.GetTile(
                _position + _possibleMoves[(direction + i) % 4]);
            if (TargetTile != null && !TargetTile.IsStaticTile())
            {
                TileObject occupiedTileObject = TargetTile.GetOccupiedTileObject();
                if (occupiedTileObject != null)
                {
                    if (occupiedTileObject.IsStaticObject())
                    {
                        // Enemy encountered static object.
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private void MovingUpdate()
    {
        if (moveTime >= 0.5 / movementSpeed)
        {
            MoveToTile(TargetTile, WorldMoveStep * 2);
        } else if (!_stoppedMoving)
        {
            _stoppedMoving = true;
            transform.position = TargetTile.transform.position;
            _occupiedTile.ClearTileObject();
            _occupiedTile = TargetTile;
            _position = _occupiedTile.GetGridPosition();
        }else if(moveTime <= 0)
        {
            ChangeState(chasing ? States.Chasing : States.Idle);
        }
        moveTime -= Time.deltaTime;
    }

    private void AttackingUpdate()
    {
        if (attackTime <= 0)
        {
            TileObject occupiedTileObject = TargetTile.GetOccupiedTileObject();
            if (occupiedTileObject != null && occupiedTileObject.CompareTag("Player"))
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
                        chasing = false;
                        ChangeState(States.Idle);
                    }
                }
                else
                {
                    chasing = false;
                    ChangeState(States.Idle);
                }

            }
            else
            {
                ChangeState(States.Chasing);
            }
        }
        // attackingTime decrement is handled in Update
    }

    private void ChasingUpdate()
    {
        if (chasingTime <= 0)
        {
            if (_playerController.GetUnitStatus().CanBeAttacked() && chasing)
            {
                if (IsPlayerInRange(unitStats.attackRange))
                {
                    TargetTile = boardController.GetTile(_lastPlayerPosition);
                    ChangeState(States.Attacking);
                } else if (IsPlayerInRange(chasingRange))
                {
                    Chase();
                }
                else
                {
                    chasing = false;
                    ChangeState(States.Idle);
                }
            }
            else
            {
                chasing = false;
                ChangeState(States.Idle);
            }
        }
        // chasingTime decrement is handled in Update.
    }

    /**
     * Chases after the player, set's targetTile if can find path to player.
     */
    private void Chase()
    {
        if (FindPathToPlayer())
        {
            if (TargetTile.SetTileObject(this))
            {
                ChangeState(States.Moving);
            }
        }
        else
        {
                chasingTime = chasingCooldown;
        }
    }

    /**
     * Checks if player is within range (grid-based), sets _lastPlayerPosition.
     */
    private bool IsPlayerInRange(int range)
    {
        Tile playerTile = _playerController.GetOccupiedTile();
        if (playerTile == null)
        {
            return false;
        }
        _lastPlayerPosition = playerTile.GetGridPosition();
        int distance = Position.Distance(_lastPlayerPosition, _position);
        if (distance <= range)
        {
            // TargetTile = boardController.GetTile(_lastPlayerPosition);
            return true;
        }

        return false;
    }

    /**
     * Finds path to player, returns true if there is path to player or player is within chaseRange.
     * If path is found, set's targetTile.
     */
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
            if (current.history.Count > chasingRange)
            {
                return false;
            }
            Tile currentTile = boardController.GetTile(current.position);
            if (currentTile != null)
            {
                
                TileObject currentOccupiedObject = currentTile.GetOccupiedTileObject();
                if(currentOccupiedObject != null && currentOccupiedObject.CompareTag("Player"))
                {
                    //Found Node
                    result = current.history;
                    result.Add(current.position);
                    TargetTile = boardController.GetTile(result[1]);
                    return true;
                }
                else
                {
                    //Didn't find Node
                    for(int i = 0; i < _possibleMoves.Count; i++)
                    {
                        Tile neighborTile = boardController.GetTile(_possibleMoves[i] + current.position);
                        if (neighborTile != null && !neighborTile.IsStaticTile() &&
                            (neighborTile.GetOccupiedTileObject() == null || neighborTile.GetOccupiedTileObject().CompareTag("Player")))
                        {
                            Node currentNeighbor = new Node();
                            currentNeighbor.position = neighborTile.GetGridPosition();
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

    public override void OnTargetObjectKilled(IAttackable target)
    {
        base.OnTargetObjectKilled(target);
        chasing = false;
        ChangeState(States.Idle);
    }

    private KeyCode GetInputDirection()
    {
        Position direction = TargetTile.GetGridPosition() - _position;

        if (direction.Equals(Position.Up))
        {
            return KeyCode.W;
        }
        if (direction.Equals(Position.Right))
        {
            return KeyCode.D;

        }
        if (direction.Equals(Position.Down))
        {
            return KeyCode.S;

        }
        if (direction.Equals(Position.Left))
        {
            return KeyCode.A;
        }

        return 0;
    }
    
    protected override void ChangeState(States newState)
    {
        switch ((int) newState)
        {
            case 0: // IDLE
            {
                IndicatorController.ClearIndicator();
                if (_playerController.GetUnitStatus().CanBeAttacked() && IsPlayerInRange(agroRange))
                {
                    ChangeState(States.Chasing);
                    return;
                }
                break;
            }
            case 1: // MOVING
            {
                IndicatorController.SetIndicator(GetInputDirection(), IndicatorState.Moving);
                moveTime = 1.0f / movementSpeed;
                _stoppedMoving = false;
            }
                break;
            case 2: // ATTACKING
            {
                IndicatorController.SetIndicator(GetInputDirection(), IndicatorState.Attacking);
                attackTime = attackCooldown;
            }
                break;
            case 3: // DISABLED
            {
                
            }
                break;
            case 4: // CHASING
            {
                IndicatorController.ClearIndicator();
                chasing = true;
            }
                break;
        }

        currentState = (int) newState;
    }
    
    private class Node
    {
        public Position position;
        public List<Position> history;
        
    }
}
