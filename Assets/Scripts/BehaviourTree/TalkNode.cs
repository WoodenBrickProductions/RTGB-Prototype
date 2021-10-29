using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class TalkNode : ActionNode
{
    public string[] textLines;
    public float readSpeed = 2;
    public int current;
    
    private UIController uiController;
    private float startTime;
    
    protected override void OnStart()
    {
        uiController = UIController.uiController;
        if (textLines.Length == 0)
        {
            return;
        }
        
        uiController.StartDialogue(gameObject.name);
        uiController.ShowDialogue(textLines[current++]);
        startTime = Time.time;
    }

    protected override void OnStop()
    {
        uiController.StopDialogue();
    }

    protected override State OnUpdate()
    {
        if (Time.time - startTime > readSpeed)
        {
            if (current == textLines.Length)
            {
                return State.Success;
            }
            uiController.ShowDialogue(textLines[current++]);
            startTime = Time.time;
        }

        return State.Running;
    }

    private void OnValidate()
    {
        if (textLines.Length != 0)
        {
            textLines[0] = description;
        }
    }
}
