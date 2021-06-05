using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private TileObject _occupiedTileObject;
    private Position _position;
    
    public TileObject GetOccupiedTileObject()
    {
        return _occupiedTileObject;
    }

    public void SetTileObject(TileObject occupiedObject)
    {
        _occupiedTileObject = occupiedObject;
    }

    public void ClearTileObject()
    {
        _occupiedTileObject = null;
    }
    
    public Position GetPosition()
    {
        return _position;
    }

    public void SetPosition(Position position)
    {
        _position = position;
    }
}
