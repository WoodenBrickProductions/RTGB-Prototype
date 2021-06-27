using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TileMap tileMap = (TileMap) target;
    
        GUILayout.Label("Perlin seed: " + tileMap.GetPerlinNoise());
        
        if (GUILayout.Button("Generate tilemap"))
        {
            tileMap.GenerateTileMap();
        }
        
        if (GUILayout.Button("Clear tilemap"))
        {
            tileMap.ClearTileMap();
        }
    }
}
