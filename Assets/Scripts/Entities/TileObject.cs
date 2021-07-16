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
        if (_occupiedTile == null)
        {
            boardController.InitializePosition(this);
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
}
