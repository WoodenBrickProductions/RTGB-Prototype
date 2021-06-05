using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [SerializeField] protected bool staticObject;
    protected Tile _occupiedTile;
    protected Position _position;
    [SerializeField] protected BoardController boardController;

    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        boardController = BoardController._boardController;
        boardController.InitializePosition(this);
        _occupiedTile = boardController.GetTile(_position);
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
}
