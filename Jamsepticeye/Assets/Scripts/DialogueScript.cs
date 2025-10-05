using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textComponent;

    [Header("Text")]
    public string[] lines;
    [Tooltip("Seconds per character")]
    public float textSpeed = 0.03f;
    public float autoNextDelay = 0.6f;     // minimum idle delay after text finishes typing
    public bool useUnscaledTime = true;    // type during paused time if true

    [Header("Voice")]
    public AudioSource voiceSource;         // assign an AudioSource (no clip needed in Inspector)
    public AudioClip[] voiceClips;          // same length/order as 'lines'
    public bool waitForVoiceToFinish = true; // auto-advance waits for voice end
    public float postVoicePause = 0.25f;    // small pause after voice ends

    private int index;
    private Coroutine typingCR;
    private Coroutine autoAdvanceCR;

    void Start()
    {
        // start hidden/empty; you'll typically call PlaySequence(...) to drive this
        if (textComponent) textComponent.text = string.Empty;

        // sanity warning (only if both arrays exist but differ)
        if (voiceClips != null && voiceClips.Length > 0 && lines != null && lines.Length > 0 && voiceClips.Length != lines.Length)
        {
            Debug.LogWarning("[DialogueScript] voiceClips length does not match lines length.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (typingCR != null)
            {
                // Instantly reveal without stomping text
                InstantReveal();
                StartIdleAutoAdvance();
            }
            else
            {
                // Fully shown -> stop voice and advance immediately
                CancelIdleAutoAdvance();
                StopVoice();
                NextLine();
            }
        }
    }

    void StartTypingCurrentLine()
    {
        // Ensure the panel is active first
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        CancelIdleAutoAdvance();

        if (typingCR != null) StopCoroutine(typingCR);
        typingCR = StartCoroutine(StartTypingDeferred());
    }

    IEnumerator StartTypingDeferred()
    {
        // Let the Canvas/Mask/Layout rebuild this frame
        yield return null;

        // If you use masks/auto layout, force a rebuild
        Canvas.ForceUpdateCanvases();

        // Assign full line once; reveal with maxVisibleCharacters
        string line = lines[index] ?? string.Empty;
        textComponent.enableWordWrapping = false;                 // optional safety
        textComponent.overflowMode = TextOverflowModes.Overflow;  // let panel clip, not TMP
        textComponent.text = line;
        textComponent.maxVisibleCharacters = 0;

        // Build TMP geometry now that the object is active & canvas updated
        textComponent.ForceMeshUpdate();

        // Start voice (if any)
        PlayVoice(index);

        // Now run the typewriter
        typingCR = StartCoroutine(TypeLine());
    }


    IEnumerator TypeLine()
    {
        // Give TMP one frame on the very first ever use (extra safe)
        yield return null;

        textComponent.ForceMeshUpdate();
        int total = textComponent.textInfo.characterCount;

        float speed = Mathf.Max(0.0001f, textSpeed);

        if (useUnscaledTime || Time.timeScale == 0f) // <-- auto-detect paused state
        {
            float t = 0f;
            int visible = 0;
            while (visible < total)
            {
                t += Time.unscaledDeltaTime;
                while (t >= speed && visible < total)
                {
                    visible++;
                    textComponent.maxVisibleCharacters = visible;
                    t -= speed;
                }
                yield return null;
            }
        }
        else
        {
            var wait = new WaitForSeconds(speed);
            for (int visible = 1; visible <= total; visible++)
            {
                textComponent.maxVisibleCharacters = visible;
                yield return wait;
            }
        }

        typingCR = null;
        StartIdleAutoAdvance();
    }

    void InstantReveal()
    {
        if (typingCR != null)
        {
            StopCoroutine(typingCR);
            typingCR = null;
        }
        textComponent.maxVisibleCharacters = int.MaxValue;
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
        if (useUnscaledTime)
        {
            float t = 0f; while (t < autoNextDelay) { if (typingCR != null) yield break; t += Time.unscaledDeltaTime; yield return null; }
        }
        else
        {
            float t = 0f; while (t < autoNextDelay) { if (typingCR != null) yield break; t += Time.deltaTime; yield return null; }
        }

        // 2) Optionally wait for voice to finish
        if (waitForVoiceToFinish && voiceSource != null && voiceSource.isPlaying)
        {
            while (voiceSource.isPlaying) { yield return null; }
            if (postVoicePause > 0f)
            {
                if (useUnscaledTime)
                {
                    float t2 = 0f; while (t2 < postVoicePause) { t2 += Time.unscaledDeltaTime; yield return null; }
                }
                else
                {
                    yield return new WaitForSeconds(postVoicePause);
                }
            }
        }

        // 3) Only auto-advance if we're still on same line and not mid-typing
        if (typingCR == null)
        {
            NextLine();
        }
        autoAdvanceCR = null;
    }

    void NextLine()
    {
        CancelIdleAutoAdvance();

        if (lines != null && index < lines.Length - 1)
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

    // === Public entry point: play any sequence on demand ===
    public void PlaySequence(DialogueSequence seq, AudioSource voiceSourceOpt = null)
    {
        if (seq == null)
        {
            Debug.LogWarning("[DialogueScript] PlaySequence called with null sequence.");
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        // Replace data from the asset
        this.lines = seq.lines;

        // If provided, wire voice clips and options
        if (seq.voiceClips != null && seq.voiceClips.Length == seq.lines.Length)
            this.voiceClips = seq.voiceClips;
        else
            this.voiceClips = null; // safe if you don't use voice for this sequence

        this.waitForVoiceToFinish = seq.waitForVoiceToFinish;
        this.postVoicePause = seq.postVoicePause;

        if (voiceSourceOpt != null) this.voiceSource = voiceSourceOpt;

        // Restart playback cleanly
        StopAllCoroutines();
        index = 0;

        if (textComponent)
        {
            textComponent.text = string.Empty;
            textComponent.maxVisibleCharacters = 0;
        }

        typingCR = null;
        autoAdvanceCR = null;

        StartTypingCurrentLine();
    }
}
