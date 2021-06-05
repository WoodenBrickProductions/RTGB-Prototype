using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [SerializeField] private Tile baseTile;
    [SerializeField] private int xSize = 1;
    [SerializeField] private int ySize = 1;
    [SerializeField] private float worldSpacing = 1;
    private Tile[,] tileMatrix;

    private void Start()
    {
        tileMatrix = new Tile[xSize, ySize];
    }

    public void GenerateTileMap(Vector3 center)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                tileMatrix[i, j] = Instantiate(baseTile, center + new Vector3(i * worldSpacing, 0, j * worldSpacing), Quaternion.identity, transform);
                tileMatrix[i, j].SetPosition(new Position(i, j));
            }
        }
    }

    public float GetWorldTileSpacing()
    {
        return worldSpacing;
    }
    
    public Tile GetTile(Position position)
    {
        if (position.x >= 0 && position.x < xSize && position.y >= 0 && position.y < ySize)
        {
            return tileMatrix[position.x, position.y];
        }
        else
        {
            return null;
        }
    }
}
