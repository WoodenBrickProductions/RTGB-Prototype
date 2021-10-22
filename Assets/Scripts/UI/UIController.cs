using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController uiController;
    // Start is called before the first frame update

    private HealthBarScript _playerHealthBar;
    private EnemyUIScript _enemyHealthBar;
    private DialogueBoxScript _dialogueBox;
    private PlayerController _playerController;
    private UnitController _currentEnemy;
    private int _currentLine;
    private Dialogue _currentDialogue;
    [SerializeField] private float _waitTime;
    private bool _continueDialogue = false;
    
    private void Awake()
    {
        uiController = this;
        _playerHealthBar = GetComponentInChildren<HealthBarScript>();
        _enemyHealthBar = GetComponentInChildren<EnemyUIScript>();
        _dialogueBox = GetComponentInChildren<DialogueBoxScript>();
    }

    private void Start()
    {
        _enemyHealthBar.gameObject.SetActive(false);
        _dialogueBox.gameObject.SetActive(false);
        _playerController = PlayerController._;
        _playerHealthBar.SetMaxHealth(_playerController.GetUnitStats().maxHealth);
    }

    private void Update()
    {
        if (_continueDialogue && _waitTime <= 0)
        {
            if (_currentLine < _currentDialogue.textLines.Length)
            {
                TextLine textLine = _currentDialogue.textLines[_currentLine++];
                _dialogueBox.SetDialogueText(textLine.text);
                _waitTime = _currentDialogue.readSpeed;
                if (textLine.interruptCallback.GetPersistentEventCount() != 0)
                {
                    _continueDialogue = false;
                    textLine.interruptCallback.Invoke();
                }
            }
            else
            {
                _continueDialogue = false;
                _dialogueBox.gameObject.SetActive(false);
            }
        }

        if (_waitTime > 0)
        {
            _waitTime -= Time.deltaTime;
        }
    }

    public void SetHealth(UnitController unitController)
    {
        switch (unitController.tag)
        {
            case "Player":
            {
                _playerHealthBar.SetHealth(unitController.GetUnitStats().currentHealth);
            }
                break;
            case "Enemy":
            {
                if (_currentEnemy != unitController)
                {
                    _enemyHealthBar.gameObject.SetActive(true);
                    _enemyHealthBar.SetName(unitController.name);
                    _currentEnemy = unitController;
                    _enemyHealthBar.SetMaxHealth(_currentEnemy.GetUnitStats().maxHealth);
                }
                _enemyHealthBar.SetHealth(unitController.GetUnitStats().currentHealth);
            }
                break;
        }
    }

    public void ShowDialogue(string text)
    {
        _dialogueBox.SetDialogueText(text);        
    }

    public void ShowDialogue(string text, int textSize)
    {
        ShowDialogue(text);
        _dialogueBox.SetTextSize(textSize);
    }
    
    public void StartDialogue(string name)
    {
        // I should switch to a thing where I send over the whole dialogue and then it does it's own thing until it reaches an interrupt, then it stops until ContinueDialogue 
        // is called, where I can pass a new callback function.
        _dialogueBox.SetName(name);
        _dialogueBox.gameObject.SetActive(true);
    }

    public void StopDialogue()
    {
        _dialogueBox.gameObject.SetActive(false);
    }
    
    public void ContinueDialogue()
    {
        _continueDialogue = true;
    }
    
    private void ShowText(Dialogue dialogue, int line)
    {
        _dialogueBox.SetDialogueText(dialogue.textLines[++line].text);
    }
    
    public void OnEnemyKilled(UnitController unitController)
    {
        _enemyHealthBar.gameObject.SetActive(false);
    }
}
