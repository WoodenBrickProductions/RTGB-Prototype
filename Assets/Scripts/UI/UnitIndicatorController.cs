using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public enum IndicatorState
{
    Attacking,
    Moving
}
public class UnitIndicatorController : MonoBehaviour
{
    private Image attackIndicator;

    private Transform cameraTransform;
    private Image _up;
    private Image _right;
    private Image _down;
    private Image _left;

    private Image _currentIndicator;
    // Start is called before the first frame update
    void Start()
    {
        Transform AttackIndicatorObject = transform.GetChild(0);
        _up = AttackIndicatorObject.GetChild(0).GetComponent<Image>();
        _right = AttackIndicatorObject.GetChild(1).GetComponent<Image>();
        _down = AttackIndicatorObject.GetChild(2).GetComponent<Image>();
        _left = AttackIndicatorObject.GetChild(3).GetComponent<Image>();
        
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.forward);
    }

    public void SetIndicator(KeyCode direction, IndicatorState state)
    {
        
        if (_currentIndicator != null)
        {
            _currentIndicator.enabled = false;
        }
        
        if (direction == KeyCode.W)
        {
            _currentIndicator = _up;
        }
        else if (direction == KeyCode.D)
        {
            _currentIndicator = _right;
        }
        else if (direction == KeyCode.S)
        {
            _currentIndicator = _down;
        }
        else if (direction == KeyCode.A)
        {
            _currentIndicator = _left;
        }
        switch (state)
        {
            case IndicatorState.Attacking:
            {
                _currentIndicator.color = Color.red;
            }
                break;
            case IndicatorState.Moving:
            {
                _currentIndicator.color = Color.white;
            }
                break;
        }
        

        _currentIndicator.enabled = true;
    }

    public void ClearIndicator()
    {
        if (_currentIndicator != null)
        {
            _currentIndicator.enabled = false;
            _currentIndicator = null;
        }
        
    }
}
