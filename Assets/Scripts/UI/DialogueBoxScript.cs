using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxScript : MonoBehaviour
{
    [SerializeField] private Text recipientName;
    [SerializeField] private Text dialogue;
    
    // Start is called before the first frame update

    private void Awake()
    {
        recipientName = transform.GetChild(0).GetComponent<Text>();
        dialogue = transform.GetChild(1).GetComponent<Text>();
    }

    private void Update()
    {
        // This needs to be here, because I need to disable it for efficiency
        
        /*if (_waitTime <= 0)
        {
            if (_currentDialogue != null && _currentLine < _currentDialogue.textLines.Length)
            {
        
            }
        }*/
    }


    public void SetName(string newName)
    {
        recipientName.text = newName;
    }

    public void SetDialogueText(string dialogueText)
    {
        dialogue.text = dialogueText;
    }

    public void SetTextSize(int size)
    {
        dialogue.fontSize = size;
    }
    
    /*
     * How do I proceed the dialogue?
     * When is it continued?
     * Do I callback when the dialogue is over?
     */
    
}
