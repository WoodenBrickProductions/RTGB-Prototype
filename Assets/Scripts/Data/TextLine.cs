using System;
using UnityEngine.Events;

[Serializable]
public class TextLine
{
    public string text;
    public UnityEvent interruptCallback;
}
