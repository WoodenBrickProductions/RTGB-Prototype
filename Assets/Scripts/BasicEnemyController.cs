using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : UnitController
{
    [SerializeField] private float wanderCooldown;
    [SerializeField] private float _wanderTime;

    [SerializeField] private float AttackCooldown;
    [SerializeField] private float _attackTime;

    [SerializeField] private float MoveCooldown;
    [SerializeField] private float _moveTime;

    [SerializeField] private bool _usePseudoRandom = false;

    private PseudoRandomNumberGenerator _pseudoRandomNumberGenerator;
    private bool _stoppedMoving;
    private List<Position> _possibleMoves;
    
    private bool _engaged = false;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        _pseudoRandomNumberGenerator = new PseudoRandomNumberGenerator(4);
        base.Start();
        _attackTime = AttackCooldown;
        _possibleMoves = new List<Position>();
        _possibleMoves.Add(Position.Up);
        _possibleMoves.Add(Position.Right);
        _possibleMoves.Add(Position.Down);
        _possibleMoves.Add(Position.Left);

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

        if (_attackTime > 0)
        {
            _attackTime -= Time.deltaTime;
        }
    }

    private void IdleUpdate()
    {
        if (!_engaged && _wanderTime <= 0)
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
            ChangeState(States.Idle);
        }
                    

                    
        _moveTime -= Time.deltaTime;
    }

    private void AttackingUpdate()
    {
        
    }

    public bool IsEngaged()
    {
        return _engaged;
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
                        // TODO Implement attacking
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
            case 2:
            {
                _currentState = 2;
            }
                break;
        }
    }
}
