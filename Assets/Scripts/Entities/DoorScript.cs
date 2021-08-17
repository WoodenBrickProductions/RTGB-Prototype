using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum function
{
    Open,
    Close,
    Toggle
}

public class DoorScript : MonoBehaviour
{
    [SerializeField] private AreaTrigger[] areaTriggers;
    [SerializeField] private function doorFunction;
    [SerializeField] private bool opened = false;
    private Tile _doorTile;
    
    private TileObject _doorObject;
    // Start is called before the first frame update
    
    void Start()
    {
        _doorObject = transform.GetChild(0).GetComponent<TileObject>();
        _doorObject.gameObject.SetActive(true);
        foreach (var trigger in areaTriggers)
        { 
            trigger.NotifyEntityEnteredHandler += DoorFunction;
        }
        _doorTile = _doorObject.GetOccupiedTile();
        if (opened)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }
    
    private void DoorFunction(TileObject sender)
    {
        switch (doorFunction)
        {
            case function.Open:
                OpenDoor();
                break;
            case function.Close:
                CloseDoor();
                break;
            case function.Toggle:
                if (opened)
                {
                    CloseDoor();
                }
                else
                {
                    OpenDoor();
                }
                break;
        }
    }
    
    private void OpenDoor()
    {
        _doorObject.GetOccupiedTile().ClearTileObject();
        _doorObject.ClearOccupiedTile();
        _doorObject.gameObject.SetActive(false);
        opened = true;
    }

    private void CloseDoor()
    {
        print("Closing door");
        
        if (_doorTile.SetTileObject(_doorObject))
        {
            _doorObject.SetOccupiedTile(_doorTile);
            _doorObject.gameObject.SetActive(true);
            opened = false;
        }
        else
        {
            print("Can't close door " + gameObject + " : object in the way");
        }
    }

    private void OnValidate()
    {
        if (_doorTile != null)
        {
            if (opened)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
        else
        {
        }
    }
}
