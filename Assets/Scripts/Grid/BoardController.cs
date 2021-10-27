using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    private float worldTileSpacing;
    public static BoardController _boardController;
    public Blackboard globalBlackboard = new Blackboard();
    
    [SerializeField] private TileMap _tileMap; 
    
    
    // Start is called before the first frame update

    private void Awake()
    {
        _boardController = this;
        worldTileSpacing = _tileMap.GetWorldTileSpacing();
        if (_tileMap.IsTileMapGenerated())
        {
            // Replace with logging
            print("Loading tilemap...");
            _tileMap.LoadTileMap();
        }
        else
        {
            // Replace with logging
            print("Generating tilemap...");
            _tileMap.GenerateTileMap();
        }
        _tileMap.SpawnEnemies();
    }

    void Start()
    {
        
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
