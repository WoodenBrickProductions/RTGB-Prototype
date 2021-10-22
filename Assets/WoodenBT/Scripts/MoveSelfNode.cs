using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelfNode : ActionNode
{
    public Vector3 direction = Vector3.up;
    public float force = 1;
    public float duration = 1;
    public GameObject targetGameObject;
    private Rigidbody body;
    private float startTime;
    
    protected override void OnStart()
    {
        body = gameObject.GetComponent<Rigidbody>();
        body.AddForce(direction * force);
    }

    protected override void OnStop()
    {
        body.AddForce(-direction * force);
    }

    protected override State OnUpdate()
    {
        if (Time.time - startTime > duration)
        {
            return State.Success;
        }

        return State.Running;
    }
}