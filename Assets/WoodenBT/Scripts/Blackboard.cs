using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Blackboard
{
    public Dictionary<string, bool> conditions = new Dictionary<string, bool>();
    public Dictionary<string, float> floatValues = new Dictionary<string, float>();
    public Dictionary<string, Vector3> vector3s = new Dictionary<string, Vector3>();
    public Dictionary<string, Position> positions = new Dictionary<string, Position>();
    public Dictionary<string, TileObject> objects = new Dictionary<string, TileObject>();
}
