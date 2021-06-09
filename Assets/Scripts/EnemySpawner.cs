using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class EnemySpawner
{
    public UnitController enemy;
    [Range(0,1)] public float spawnRate;
}
