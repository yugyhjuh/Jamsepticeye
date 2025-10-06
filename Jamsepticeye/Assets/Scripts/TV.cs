using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : MonoBehaviour
{
    public AudioSource audioSource;  // assign in Inspector
    public AudioClip clip;           // assign in Inspector
    public float delay = 7f;         // wait time in seconds

    private void Start()
    {
        StartCoroutine(PlayAfterDelay());
    }

    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay); // wait 7 seconds
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
