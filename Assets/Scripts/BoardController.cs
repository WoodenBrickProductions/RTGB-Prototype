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
        Vector3 worldPosition = tileObject.transform.position;
        Position position = new Position((int) (worldPosition.x / worldTileSpacing), (int) (worldPosition.z / worldTileSpacing));
        Tile tile = GetTile(position);

        if (tile == null)
        {
            print("Can't place object " + tileObject + " : object outside tilemap");
            Destroy(tileObject.gameObject);
            return;
        }

        if (tile.GetOccupiedTileObject() != null)
        {
            print("Can't place object " + tileObject + " : tile already occupied");
            Destroy(tileObject.gameObject);
            return;
        }
        
        if (tile.SetTileObject(tileObject))
        {
            tileObject.SetPosition(position);
            tileObject.transform.position = new Vector3(position.x * worldTileSpacing, 0, position.y * worldTileSpacing);
            tileObject.SetOccupiedTile(tile);
        }
        else
        {
            print("Can't place object " + tileObject + " : tile is static");
            position = _tileMap.GetValidTile();
            tileObject.SetPosition(position);
            tileObject.transform.position = new Vector3(position.x * worldTileSpacing, 0, position.y * worldTileSpacing);
            tileObject.SetOccupiedTile(tile);
                        
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
