using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public Action<TileObject> NotifyEntityEnteredHandler;
    
    [SerializeField] private int xSize, ySize;
    [SerializeField] private string[] tags;
    
    private Position _position;
    private Tile[,] _tiles;
    private BoardController _boardController;
    
    private void Awake()
    {
        if (xSize == 0 || ySize == 0)
        {
            print("Can't create AreaTrigger: size too small");
            Destroy(this);
            enabled = false;
            return;
        }
        _tiles = new Tile[xSize, ySize];
        Vector3 worldPos = transform.localPosition;
        _position = new Position((int) worldPos.x, (int) worldPos.z);
        transform.position = new Vector3(_position.x, 0, _position.y);
        _boardController = BoardController._boardController;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                Tile tile = _boardController.GetTile(new Position(_position.x + i, _position.y + j));
                if (!tile.IsStaticTile())
                {
                    _tiles[i, j] = tile;
                    tile.NotifyTileEnteredHandler += OnCustomTriggerEnter;
                }
            }
        }
    }

    private void OnCustomTriggerEnter(TileObject tileObject)
    {
        foreach (var tag in tags) //Need to change String to smth more proper
        {
            if (tileObject.CompareTag(tag))
            {
                NotifyEntityEnteredHandler(tileObject);
                print("Player has entered the trigger!");
            }
        }
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetGridPosition(Position position)
    {
        _position = position;
    }
}
