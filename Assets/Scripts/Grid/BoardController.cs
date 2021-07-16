﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    private float worldTileSpacing;
    public static BoardController _boardController;
    [SerializeField] private TileMap _tileMap; 
    
    
    // Start is called before the first frame update

    private void Awake()
    {
        _boardController = this;
    }

    void Start()
    {
        worldTileSpacing = _tileMap.GetWorldTileSpacing();
        _tileMap.GenerateTileMap();
        _tileMap.SpawnEnemies();
    }

    public float GetWorldTileSpacing()
    {
        return worldTileSpacing;
    }

    public void InitializePosition(TileObject tileObject)
    {
        if (!_tileMap.InitializePosition(tileObject))
        {
            if(tileObject.CompareTag("Player"))
            {
                Tile tile = _tileMap.GetValidTile();
                if (tile.SetTileObject(tileObject))
                {
                    tileObject.transform.position = tile.transform.position;
                    tileObject.SetOccupiedTile(tile);
                }
                else
                {
                    print("Couldn't place " + tileObject + ": didn't get valid tile");
                }
            }
            else
            {
                print("Couldn't place " + tileObject + ", destroying");
                Destroy(tileObject.gameObject);
            }
          
        }
    }

    public Tile GetTile(Position position)
    {
        return _tileMap.GetTile(position);
    }
}
