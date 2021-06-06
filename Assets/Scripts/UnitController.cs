using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    Idle = 0,
    Moving = 1,
    Attacking = 2,
    Disabled = 3
}

public class UnitController : TileObject
{
    [SerializeField] protected float movementSpeed = 1;

    protected void MoveToTile(Tile tile, float worldMovementStep)
    {
        // TODO is this optimal?
        transform.position = Vector3.MoveTowards(transform.position, tile.transform.position, movementSpeed * worldMovementStep * Time.deltaTime);
    }
}


