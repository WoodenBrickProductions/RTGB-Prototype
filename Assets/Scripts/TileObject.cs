using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [SerializeField] protected bool staticObject;
    protected Tile _occupiedTile;
    protected Position _position = null;
    protected BoardController boardController;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boardController = BoardController._boardController;
        if (_position == null)
        {
            boardController.InitializePosition(this);
        }
    }

    public void SetPosition(Position position)
    {
        _position = position;
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
    }
}
