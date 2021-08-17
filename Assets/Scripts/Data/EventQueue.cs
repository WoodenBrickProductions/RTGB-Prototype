using System;
using System.Collections.Generic;
using UnityEngine;



public class EventQueue : MonoBehaviour
{
    public struct CustomEventRequest
    {
        CustomEvent customEvent;
        GameObject sender;
    };
    
    public delegate void CustomEvent(GameObject sender);
    
    public static EventQueue current;
    private Queue<CustomEvent> eventQueue;
    private Queue<GameObject> senders;

    private void Awake()
    {
        current = this;
        eventQueue = new Queue<CustomEvent>();
        senders = new Queue<GameObject>();
    }

    private void Update()
    {
        if (eventQueue.Count != 0)
        {
            eventQueue.Dequeue()(senders.Dequeue());
        }
    }

    public bool AddEventRequest(CustomEvent eventCallback, GameObject sender)
    {
        if (eventQueue.Contains(eventCallback))
        {
            // Handle identical events
        }
        else
        {
            eventQueue.Enqueue(eventCallback);
            senders.Enqueue(sender);
        }
        return true;
    }
}