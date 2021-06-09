using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    private float worldTileSpacing;
    public static BoardController _boardController;
    private float _perlinSeed;
    [SerializeField] private TileMap _tileMap; 
    
    
    // Start is called before the first frame update

    private void Awake()
    {
        _boardController = this;
    }

    void Start()
    {
        _perlinSeed = Random.value * 1000;
        worldTileSpacing = _tileMap.GetWorldTileSpacing();
        _tileMap.GenerateTileMap();
        _tileMap.SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
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
            print("Couldn't place " + tileObject + ", destroying");
            Destroy(tileObject);
        }
    }

    public void InitializePosition(PlayerController player)
    {
        if (!_tileMap.InitializePosition(player))
        {
            Tile tile = _tileMap.GetValidTile();
            if (tile.SetTileObject(player))
            {
                player.SetPosition(tile.GetPosition());
                player.transform.position = tile.transform.position;
                player.SetOccupiedTile(tile);
                tile.SetTileObject(player);
            }
            
            
            
            print("Couldn't place " + player + ": didn't get valid tile");
        }
    }

    public Tile GetTile(Position position)
    {
        return _tileMap.GetTile(position);
    }

    public float GetGenerationSeed()
    {
        return _perlinSeed;
    }
}
