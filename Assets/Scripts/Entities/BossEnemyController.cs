using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class BossEnemyController : UnitController
{
    [SerializeField] private AIStats _aiStats;

    public BehaviourTree tree;
    public Blackboard blackboard;
    


    
    // ----------------------------------------------------------
    protected PlayerController _playerController;
    protected PseudoRandomNumberGenerator pseudoRandomNumberGenerator;
        
    protected List<Position> _directionalMoves;
    protected bool _stoppedMoving;
    protected Position lastPlayerPosition;

    private bool _playerEntered = false;
    private Action _currentAction;
    
    private float waitTime; // ??
    
    protected override void Start()
    {
        base.Start();
        _playerController = PlayerController._;
    
        // Data
        tag = "Enemy";
        name = "BossenSchmurtz";
        
        // Parameter initialization
        _aiStats.wanderTime = _aiStats.wanderCooldown + Random.value;
        pseudoRandomNumberGenerator = new PseudoRandomNumberGenerator(4);
        attackTime = attackCooldown;
        blackboard = new Blackboard();    
        
        _directionalMoves = new List<Position>();
        _directionalMoves.Add(Position.Up);
        _directionalMoves.Add(Position.Right);
        _directionalMoves.Add(Position.Down);
        _directionalMoves.Add(Position.Left);
        _playerController.OnPauseAll += Pause; // ??
        
        // Behaviour Tree Initialization
        tree = tree.Clone();
        tree.Bind(gameObject, blackboard);
        
        SetupActOne();
    }

    // -------------------------------------------------------------
    // Act 1
    // -------------------------------------------------------------
    
    [Header("Act 1")]
    public AreaTrigger roomEnterTrigger;
    public AreaTrigger doorwayTrigger;
    public AreaTrigger doorOpenTrigger;
    public AreaTrigger doorCloseTrigger1;
    public AreaTrigger doorCloseTrigger2;
    
    
    private void SetupActOne()
    {
        blackboard.conditions.Add("PlayerSteppedOnDoorClose", false);
        blackboard.conditions.Add("PlayerEnteredRoom", false);
        blackboard.conditions.Add("PlayerEnteredDoorway", false);
        
        doorwayTrigger.NotifyEntityEnteredHandler += OnPlayerEnteredDoorway;
        roomEnterTrigger.NotifyEntityEnteredHandler += OnPlayerEnteredRoom;
        doorOpenTrigger.NotifyEntityEnteredHandler += OnPlayerSteppedOnDoorOpen;
        doorCloseTrigger1.NotifyEntityEnteredHandler += OnPlayerSteppedOnDoorClose;
        doorCloseTrigger2.NotifyEntityEnteredHandler += OnPlayerSteppedOnDoorClose;
    }

    private void OnPlayerSteppedOnDoorOpen(TileObject obj)
    {
        blackboard.conditions["PlayerSteppedOnDoorOpen"] = true;
    }

    private void OnPlayerEnteredRoom(TileObject obj)
    {
        blackboard.conditions["PlayerEnteredRoom"] = true;
    }

    private void OnPlayerEnteredDoorway(TileObject obj)
    {
        blackboard.conditions["PlayerEnteredDoorway"] = true;
    }

    void OnPlayerSteppedOnDoorClose(TileObject tileObject)
    {
        blackboard.conditions["PlayerSteppedOnDoorClose"] = true;
    }
    
    // -------------------------------------------------------------
    // Act 2
    // -------------------------------------------------------------

    [Header("Act 2")] 
    private AreaTrigger trigger;
    
    private void SetupActTwo()
    {
        
    }
    
    void Update()
    {
        switch (currentState)
        {
            case States.Paused:
                break;
            case States.Idle:
                IdleUpdate();
                break;
            case States.Moving:
                MovingUpdate();
                break;
            case States.Attacking:
                AttackingUpdate();
                break;
            case States.Disabled:
                break;
            case States.Chasing:
                ChasingUpdate();
                break;
            case States.Talking:
                break;
            case States.Searching:
                break;
            case States.Waiting:
                WaitingUpdate();
                break;
            case States.Acting:
                ActingUpdate();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (attackTime > 0)
        {
            attackTime -= Time.deltaTime;
        }
        
        if (_aiStats.chasingTime > 0)
        {
            _aiStats.chasingTime -= Time.deltaTime;
        }
    }
    public void CommencePlayerEnteredRoom(TileObject sender)
    {
        ChangeState(States.Talking);
        uiController.StartDialogue(name);
    }

    private void WaitingUpdate()
    {
        if (IsPlayerInRange(_aiStats.talkRange))
        {
            // NotifyPlayerInTalkRangeHandler?.Invoke(this);
        }
    }
    
    private void IdleUpdate()
    {
        if (IsPlayerInRange(15))
        {
            ChangeState(States.Acting);
        }
    }

    private void ActingUpdate()
    {
        tree.Update();
    }
    
    
    /**
     * Set next possible tile to wander to, set's TargetTile
     */
    private bool Wander()
    {
        int randomValue;
        if (_aiStats.usePseudoRandom)
        {
            randomValue = pseudoRandomNumberGenerator.GetPseudoRandomNumber();
        }
        else
        {
            randomValue = (int) (Random.value * 4);
        }
        
        int direction = randomValue;
        for(int i = 0; i < 4; i++)
        {
            TargetTile = boardController.GetTile(
                _position + _directionalMoves[(direction + i) % 4]);
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
            ChangeState(_aiStats.chasing ? States.Chasing : States.Idle);
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
                        _aiStats.chasing = false;
                        ChangeState(States.Idle);
                    }
                }
                else
                {
                    _aiStats.chasing = false;
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
        if (_aiStats.chasingTime <= 0)
        {
            if (_playerController.GetUnitStatus().CanBeAttacked() && _aiStats.chasing)
            {
                if (IsPlayerInRange(unitStats.attackRange))
                {
                    TargetTile = boardController.GetTile(lastPlayerPosition);
                    ChangeState(States.Attacking);
                } else if (IsPlayerInRange(_aiStats.chasingRange))
                {
                    Chase();
                }
                else
                {
                    _aiStats.chasing = false;
                    ChangeState(States.Idle);
                }
            }
            else
            {
                _aiStats.chasing = false;
                ChangeState(States.Idle);
            }
        }
        // _aiStats.chasingTime decrement is handled in Update.
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
                _aiStats.chasingTime = _aiStats.chasingCooldown;
        }
    }

    /**
     * Checks if player is within range (grid-based), sets lastPlayerPosition.
     */
    private bool IsPlayerInRange(int range)
    {
        Tile playerTile = _playerController.GetOccupiedTile();
        if (playerTile == null)
        {
            return false;
        }
        lastPlayerPosition = playerTile.GetGridPosition();
        int distance = Position.Distance(lastPlayerPosition, _position);
        if (distance <= range)
        {
            // TargetTile = boardController.GetTile(lastPlayerPosition);
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
            if (current.history.Count > _aiStats.chasingRange)
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
                    for(int i = 0; i < _directionalMoves.Count; i++)
                    {
                        Tile neighborTile = boardController.GetTile(_directionalMoves[i] + current.position);
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
        _aiStats.chasing = false;
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
        switch (currentState)
        {
            case States.Paused:
                break;
            case States.Idle:
                IndicatorController.ClearIndicator();
                if (_playerController.GetUnitStatus().CanBeAttacked() && IsPlayerInRange(_aiStats.agroRange))
                {
                    ChangeState(States.Chasing);
                    return;
                }
                break;
            case States.Moving:
                IndicatorController.SetIndicator(GetInputDirection(), IndicatorState.Moving);
                moveTime = 1.0f / movementSpeed;
                _stoppedMoving = false;
                break;
            case States.Attacking:
                IndicatorController.SetIndicator(GetInputDirection(), IndicatorState.Attacking);
                attackTime = attackCooldown;
                break;
            case States.Disabled:
                break;
            case States.Chasing:
                IndicatorController.ClearIndicator();
                _aiStats.chasing = true;
                break;
            case States.Talking:
                break;
            case States.Searching:
                break;
            case States.Waiting:
                break;
            case States.Acting:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
            

        currentState = newState;
    }

    private class Node
    {
        public Position position;
        public List<Position> history;
        
    }
}
