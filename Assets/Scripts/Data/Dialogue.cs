using System;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Header("Dialogue")]
    public string name;
    public string description;
    public float readSpeed;
    public TextLine[] textLines; 
}
