using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textComponent;

    [Header("Text")]
    public string[] lines;
    public float textSpeed = 0.03f;
    public float autoNextDelay = 0.6f;     // minimum idle delay after text finishes typing

    [Header("Voice")]
    public AudioSource voiceSource;        // assign an AudioSource (no clip needed in Inspector)
    public AudioClip[] voiceClips;         // same length/order as 'lines'
    public bool waitForVoiceToFinish = true; // auto-advance waits for voice end
    public float postVoicePause = 0.25f;   // small pause after voice ends

    private int index;
    private Coroutine typingCR;
    private Coroutine autoAdvanceCR;

    void Start()
    {
        textComponent.text = string.Empty;

        if (voiceClips != null && voiceClips.Length > 0 && voiceClips.Length != lines.Length)
            Debug.LogWarning("voiceClips length does not match lines length. Missing clips will be skipped.");

        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // If we're still typing, reveal instantly but keep voice playing
            if (typingCR != null)
            {
                StopCoroutine(typingCR);
                typingCR = null;
                textComponent.text = lines[index];
                StartIdleAutoAdvance();
            }
            else
            {
                // Already fully shown: skip to next line immediately
                CancelIdleAutoAdvance();
                StopVoice();
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
        CancelIdleAutoAdvance();
        textComponent.text = string.Empty;

        // Start voice for this line (if exists)
        PlayVoice(index);

        typingCR = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        typingCR = null;           // finished typing
        StartIdleAutoAdvance();    // now wait to auto-advance
    }

    void StartIdleAutoAdvance()
    {
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
        // 1) Always wait at least 'autoNextDelay' after the text is fully visible
        float t = 0f;
        while (t < autoNextDelay)
        {
            if (typingCR != null) yield break; // typing restarted (safety)
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // 2) Optionally wait for voice to finish
        if (waitForVoiceToFinish && voiceSource != null && voiceSource.isPlaying)
        {
            // Wait until the currently playing line's clip is done
            while (voiceSource.isPlaying) { yield return null; }
            if (postVoicePause > 0f) yield return new WaitForSeconds(postVoicePause);
        }

        // 3) Only auto-advance if we're still on the same line and fully shown
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
            StopVoice();
            gameObject.SetActive(false);
        }
    }

    // --- Voice helpers ---

    void PlayVoice(int i)
    {
        if (voiceSource == null || voiceClips == null || i < 0 || i >= voiceClips.Length) return;
        var clip = voiceClips[i];
        if (clip == null) return;

        voiceSource.Stop();      // ensure clean start
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();
    }
}
