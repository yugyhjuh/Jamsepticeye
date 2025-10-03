using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.03f;
    public float autoNextDelay = 1f; // time to wait before auto-advance when a line is fully shown

    private int index;
    private Coroutine typingCR;
    private Coroutine autoAdvanceCR;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // If we're still typing, reveal instantly and start the idle timer
            if (typingCR != null)
            {
                StopCoroutine(typingCR);
                typingCR = null;
                textComponent.text = lines[index];
                StartIdleAutoAdvance(); // start auto-advance after reveal
            }
            else
            {
                // Line is already fully shown: advance immediately on click
                CancelIdleAutoAdvance();
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartTypingCurrentLine();
    }

    void StartTypingCurrentLine()
    {
        CancelIdleAutoAdvance(); // ensure no stray timers
        textComponent.text = string.Empty;
        typingCR = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // Finished typing naturally
        typingCR = null;
        StartIdleAutoAdvance();
    }

    void StartIdleAutoAdvance()
    {
        // Restart the idle timer from scratch
        CancelIdleAutoAdvance();
        autoAdvanceCR = StartCoroutine(AutoAdvanceAfterDelay());
    }

    void CancelIdleAutoAdvance()
    {
        if (autoAdvanceCR != null)
        {
            StopCoroutine(autoAdvanceCR);
            autoAdvanceCR = null;
        }
    }

    IEnumerator AutoAdvanceAfterDelay()
    {
        yield return new WaitForSeconds(autoNextDelay);

        // Only auto-advance if the line is still fully shown and we haven't started typing again
        if (typingCR == null && textComponent.text == lines[index])
        {
            NextLine();
        }
        autoAdvanceCR = null;
    }

    void NextLine()
    {
        CancelIdleAutoAdvance();

        if (index < lines.Length - 1)
        {
            index++;
            StartTypingCurrentLine();
        }
        else
        {
            // End of dialogue
            gameObject.SetActive(false);
        }
    }
}
