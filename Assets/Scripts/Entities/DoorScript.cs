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

[Serializable]
class TriggerForDoor
{
    public AreaTrigger areaTrigger;
    public function doorFunction;
}

public class DoorScript : MonoBehaviour
{
    [SerializeField] private TriggerForDoor[] doorTriggers;
    [SerializeField] private bool opened = false;
    private Tile _doorTile;
    
    private TileObject _doorObject;
    // Start is called before the first frame update
    
    void Start()
    {
        _doorObject = transform.GetChild(0).GetComponent<TileObject>();
        _doorObject.gameObject.SetActive(true);
        foreach (var trigger in doorTriggers)
        { 
            trigger.areaTrigger.NotifyEntityEnteredHandler += DoorFunction(trigger.doorFunction);
        }
        _doorTile = _doorObject.GetOccupiedTile();
        if (opened)
        {
            OpenDoor(null);
        }
        else
        {
            CloseDoor(null);
        }
    }
    
    private Action<TileObject> DoorFunction(function doorFunction)
    {
        switch (doorFunction)
        {
            case function.Open:
                return OpenDoor;
            case function.Close:
                return CloseDoor;
            case function.Toggle:
                return ToggleDoor;
        }
        return null;
    }

    private void ToggleDoor(TileObject sender)  //Toggledore, the greatest contraption wizard of the land
    {
        if (opened)
        {
            CloseDoor(sender);
        }
        else
        {
            OpenDoor(sender);
        }
        opened = !opened;
    }
    
    private void OpenDoor(TileObject sender)
    {
        _doorObject.GetOccupiedTile().ClearTileObject();
        _doorObject.ClearOccupiedTile();
        _doorObject.gameObject.SetActive(false);
        opened = true;
    }

    private void CloseDoor(TileObject sender)
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
        foreach(var trigger in doorTriggers)
        {
            if (trigger.areaTrigger == null)
            {
                print("Missing trigger in " + name + "!");
            }
        }
        if (_doorTile != null)
        {
            if (opened)
            {
                OpenDoor(null);
            }
            else
            {
                CloseDoor(null);
            }
        }
        else
        {
        }
    }
}
