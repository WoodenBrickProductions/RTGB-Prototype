using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    public delegate void NotifyTileEntered(TileObject tileObject);

    
    public long time;

    public static bool GameFrozen = false;
    
    public long loadTime;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
    }

    // Update is called once per frame
    
#if UNITY_EDITOR

    private void OnPlayModeChanged(PlayModeStateChange stateChange)
    {
        switch (stateChange)
        {
            case PlayModeStateChange.ExitingEditMode:
                // save the system time when leaving the edit mode
                time = DateTime.Now.Ticks;
                break;

            case PlayModeStateChange.EnteredPlayMode:
                // save and compare the system time when entering play mode
                loadTime = DateTime.Now.Ticks;
                var difference = (loadTime - time) / TimeSpan.TicksPerMillisecond;

                Debug.Log("Load Time was: " + difference + "ms", this);
                break;
        }
    }

    private void OnEnable()
    {
        // Register to the playModestaeChanged event
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private void OnDisable()
    {
        // Unregister from the playModeStateChanged event
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

#endif
}
