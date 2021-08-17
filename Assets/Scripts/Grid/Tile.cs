using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public Action<TileObject> NotifyTileEnteredHandler;
    
    
    [SerializeField] private TileObject _occupiedTileObject;
    [SerializeField] private bool staticTile = false;
    private Position _position;
    
    private GameObject _solidModel;
    private GameObject _pitModel;

    private void Awake()
    {
        _solidModel = transform.GetChild(0).gameObject;
        _pitModel = transform.GetChild(1).gameObject;
    }

    public void Start()
    {
        SetStaticTile(staticTile);
    }

    public TileObject GetOccupiedTileObject()
    {
        return _occupiedTileObject;
    }

    public bool SetTileObject(TileObject occupiedObject)
    {
        if (!staticTile && _occupiedTileObject == null)
        {
            _occupiedTileObject = occupiedObject;
            if (NotifyTileEnteredHandler != null)
            {
                NotifyTileEnteredHandler(occupiedObject);
            }
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

    public void SetStaticTile(bool isStatic)
    {
        staticTile = isStatic;
        if (_solidModel == null || _pitModel == null)
        {
            _solidModel = transform.GetChild(0).gameObject;
            _pitModel = transform.GetChild(1).gameObject;
        }
        if (staticTile)
        {
            _solidModel.SetActive(false);
            _pitModel.SetActive(true);
        }
        else
        {
            _solidModel.SetActive(true);
            _pitModel.SetActive(false);
        }
    }
    
    public Position GetGridPosition()
    {
        return _position;
    }

    public void SetGridPosition(Position position)
    {
        _position = position;
    }

}
