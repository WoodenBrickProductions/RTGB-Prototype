﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController _cameraController;
    [SerializeField] private UnitController cameraFollow = null;
    [SerializeField] private float movementSpeed = 1;
    private bool _targetChangedPosition = false;
    private float worldSpaceStep;


    private void Awake()
    {
        _cameraController = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cameraFollow == null)
        {
            cameraFollow = PlayerController._;
        }
        movementSpeed = cameraFollow.GetMovementSpeed();
        worldSpaceStep = BoardController._boardController.GetWorldTileSpacing();
        transform.position = cameraFollow.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraFollow == null)
        {
            enabled = false;
            return;
        }
            
        // Change to listen for change?
        if (!_targetChangedPosition && transform.position != cameraFollow.transform.position)
        {
            _targetChangedPosition = true;
        }
        
        if (_targetChangedPosition && Vector3.Distance(transform.position, cameraFollow.transform.position) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, cameraFollow.transform.position, worldSpaceStep * movementSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = cameraFollow.transform.position;
            _targetChangedPosition = false;
        }
    }

    public void SetSpeed(float speed)
    {
        movementSpeed = speed;
    }
}
