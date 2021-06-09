using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour
{
    [SerializeField] private EnemySpawner[] enemies;
    [SerializeField] private Tile baseTile;
    [SerializeField] private TileObject wallObject;
    [SerializeField] private int xSize = 1;
    [SerializeField] private int ySize = 1;
    [SerializeField] private float worldSpacing = 1;
    [SerializeField] [Range(0, 1)] private float tileSpawnRate = 1;
    [SerializeField] [Range(0, 1)] private float enemySpawnRate = 0.5f;
    [SerializeField] [Range(0, 1)] private float wallSpawning = 0.0f; 
    [SerializeField] private float spawnScaling = 1;
    [SerializeField] private bool generateEnemies = false;
    private Tile[,] tileMatrix;
    private float _perlinSeed;
    private BoardController _boardController;
    
    private void Start()
    {
        tileMatrix = new Tile[xSize, ySize];
        _perlinSeed = Random.value * 1000;
    }

    public void GenerateTileMap()
    {
        _boardController = BoardController._boardController;
        _perlinSeed = _boardController.GetGenerationSeed();
        tileMatrix = new Tile[xSize, ySize];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                float spawnValue = Mathf.PerlinNoise((i + _perlinSeed) * spawnScaling, (j + _perlinSeed) * spawnScaling);
                if (spawnValue <= tileSpawnRate)
                {
                    tileMatrix[i, j] = Instantiate(
                        baseTile,
                        transform.position + new Vector3(i * worldSpacing, 0, j * worldSpacing),
                        Quaternion.identity,
                        transform.GetChild(0));
                    tileMatrix[i, j].SetPosition(new Position(i, j));
                    if (spawnValue > tileSpawnRate * (1 - wallSpawning))
                    {
                        TileObject wall = Instantiate(wallObject,
                            transform.position + new Vector3(i * worldSpacing, 0, j * worldSpacing),
                            Quaternion.identity, 
                            transform.GetChild(0));
                        InitializePosition(wall);
                    }
                }
                else
                {
                    tileMatrix[i, j] = Instantiate(
                        baseTile,
                        transform.position + new Vector3(i * worldSpacing, 0, j * worldSpacing),
                        Quaternion.identity,
                        transform.GetChild(0));
                    tileMatrix[i, j].SetStaticTile(true);
                    tileMatrix[i, j].SetPosition(new Position(i, j));
                }
            }
        }
    }

    public void SpawnEnemies()
    {
        if (generateEnemies)
        {
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    float spawnValue = Mathf.PerlinNoise((i + _perlinSeed) * spawnScaling, (j + _perlinSeed) * spawnScaling);
                    if (!tileMatrix[i, j].IsStaticTile() && tileMatrix[i, j].GetOccupiedTileObject() == null)
                    {
                        if (spawnValue <= enemySpawnRate)
                        {
                            foreach (EnemySpawner enemySpawner in enemies)
                            {
                                if (Random.value <= enemySpawner.spawnRate)
                                {
                                    TileObject enemy = Instantiate(enemySpawner.enemy,
                                        transform.position + new Vector3(i * worldSpacing, 0, j * worldSpacing),
                                        Quaternion.identity, 
                                        transform.GetChild(2));
                                    InitializePosition(enemy);
                                    break;
                                }
                            }
                        }
                    }
                }  
            }
        }
    }

    public void GenerateTile(Position position)
    {
        if (position.x >= 0 && position.x < xSize && position.y >= 0 && position.y < ySize)
        {
            tileMatrix[position.x, position.y] = Instantiate(
                baseTile,
                transform.position + new Vector3(position.x * worldSpacing, 0, position.y * worldSpacing),
                Quaternion.identity,
                transform);
            tileMatrix[position.x, position.y].SetPosition(new Position(position.x, position.y));
        }
    }

    public float GetWorldTileSpacing()
    {
        return worldSpacing;
    }

    public Tile GetValidTile()
    {
        int x = (int)(Random.value * xSize);
        int y = (int)(Random.value * ySize);

        for (int i = x; i < xSize; i++)
        {
            for (int j = y; j < ySize; j++)
            {
                Tile tile = tileMatrix[i, j];
                if (!tile.IsStaticTile() && tile.GetOccupiedTileObject() == null)
                {
                    return tileMatrix[i, j];
                }
            }
        }

        return null;
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

    public bool InitializePosition(TileObject tileObject)
    {
        Vector3 worldPosition = tileObject.transform.position;
        Position position = new Position((int) (worldPosition.x / worldSpacing), (int) (worldPosition.z / worldSpacing));
        Tile tile = GetTile(position);

        if (tile == null)
        {
            print("Can't place object " + tileObject + " : object outside tilemap");
            return false;
        }

        if (tile.IsStaticTile())
        {
            print("Can't place object " + tileObject + " : tile is static at position " + tile.GetPosition());
            return false;
        }
        
        if (tile.GetOccupiedTileObject() != null)
        {
            print("Can't place object " + tileObject + " : tile already occupied at position " + tile.GetPosition());
            return false;
        }
        
        if (tile.SetTileObject(tileObject))
        {
            tileObject.SetPosition(position);
            tileObject.transform.position = tile.transform.position;
            tileObject.SetOccupiedTile(tile);
            tile.SetTileObject(tileObject);
            return true;
        }

        print("Can't place object " + tileObject + " : tile already occupied at position " + tile.GetPosition());
        return false;
    }
}
