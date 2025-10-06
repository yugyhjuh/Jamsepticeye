using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogCombiner : MonoBehaviour
{
    public AudioSource audioSource;     // assign in Inspector
    public AudioClip[] clips;           // assign clips in order

    private void Start()
    {
        if (clips.Length > 0)
            StartCoroutine(PlayAudioSequence());
    }

    private IEnumerator PlayAudioSequence()
    {
        foreach (AudioClip clip in clips)
        {
            if (clip == null) continue;

            audioSource.clip = clip;
            audioSource.Play();

            // Wait until the clip finishes
            yield return new WaitForSeconds(clip.length);
        }

        // Optional: sequence finished
        Debug.Log("All clips played!");
    }
}
