using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{

    public DialogueScript dialogueScript;
    public DialogueSequence dialogueSequence;

    public void playSequence()
    {
        dialogueScript.PlaySequence(dialogueSequence);
    }
}
