using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _tileMap.GenerateTileMap(Vector3.zero);
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
        tileObject.SetPosition(position);
        tileObject.transform.position = new Vector3(position.x * worldTileSpacing, 0, position.y * worldTileSpacing);
        Tile tile = GetTile(position);
        tileObject.SetOccupiedTile(tile);
        tile.SetTileObject(tileObject);
    }

    public Tile GetTile(Position position)
    {
        return _tileMap.GetTile(position);
    }
}
