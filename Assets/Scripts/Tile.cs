using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileObject _occupiedTileObject;
    [SerializeField] private bool staticTile = false;
    private Position _position;
    
    public TileObject GetOccupiedTileObject()
    {
        return _occupiedTileObject;
    }

    public bool SetTileObject(TileObject occupiedObject)
    {
        if (!staticTile)
        {
            _occupiedTileObject = occupiedObject;
            return true;
        }

        return false;
    }

    public void ClearTileObject()
    {
        _occupiedTileObject = null;
    }

    public bool IsStaticTile()
    {
        return staticTile;
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
