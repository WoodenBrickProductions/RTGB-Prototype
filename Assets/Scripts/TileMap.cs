using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour
{
    [SerializeField] private Tile baseTile;
    [SerializeField] private EnemyCluster enemyCluster;
    [SerializeField] private int xSize = 1;
    [SerializeField] private int ySize = 1;
    [SerializeField] private float worldSpacing = 1;
    [SerializeField] private float tileSpawnRate = 1;
    [SerializeField] private float enemySpawnRate = 0.5f;
    [SerializeField] private float spawnScaling = 1;
    [SerializeField] private bool generateEnemies = false;
    private Tile[,] tileMatrix;
    private float _perlinSeed;

    private void Start()
    {
        tileMatrix = new Tile[xSize, ySize];
        _perlinSeed = Random.value * 1000;
    }

    public void GenerateTileMap()
    {
        _perlinSeed = BoardController._boardController.GetGenerationSeed();
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
                        transform);
                    tileMatrix[i, j].SetPosition(new Position(i, j));
                    if (generateEnemies && spawnValue <= enemySpawnRate * tileSpawnRate)
                    {
                        // TODO: Add enemies to spawn
                    }
                }
                else
                {
                    tileMatrix[i, j] = Instantiate(
                        baseTile,
                        transform.position + new Vector3(i * worldSpacing, 0, j * worldSpacing),
                        Quaternion.identity,
                        transform);
                    tileMatrix[i, j].SetStaticTile(true);
                    tileMatrix[i, j].SetPosition(new Position(i, j));
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

    public Position GetValidTile()
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
                    return tileMatrix[i, j].GetPosition();
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
}
