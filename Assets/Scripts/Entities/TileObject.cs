using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [Header("Tile Object")]
    [SerializeField] protected bool staticObject;
    [SerializeField] protected Tile _occupiedTile;
    protected Position _position = null;
    protected BoardController boardController;
    
        
    
    protected virtual void Start()
    {
        boardController = BoardController._boardController;
        if (_occupiedTile == null)
        {
            // print("_occupiedTile is null");
            boardController.InitializePosition(this);
        }
        else
        {
            // print("OccupiedTile is " + _occupiedTile);
        }
    }

    public bool IsStaticObject()
    {
        return staticObject;
    }
    
    public Tile GetOccupiedTile()
    {
        return _occupiedTile;
    }

    public void SetOccupiedTile(Tile tile)
    {
        _occupiedTile = tile;
        _position = _occupiedTile.GetGridPosition();
    }

    public void ClearOccupiedTile()
    {
        _occupiedTile.ClearTileObject();
    }

    protected virtual void SubscribeToEvents()
    {
        
    }

    public void AddMessageToQueue(string message, TileObject sender)
    {
        // Add message to queue
    }
    
}
