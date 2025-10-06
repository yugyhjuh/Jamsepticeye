using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;
using TMPro;

public class Interaction : MonoBehaviour
{
    Transform player;

    public AudioSource audioSource;

    public AudioClip sleepingClip;

    public GameObject blackScreen;

    public string nextScene;

    public ParticleSystem ps;
    public GameObject firebins;
    public AudioClip eerieClip;

    public TMP_Text yourFree;
    public TMP_Text thankYouText;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.GetChild(0);
        if (ps != null)
            ps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact(hit);
            }
        }
    }

    void Interact(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<Lift>(out Lift lift))
        {
            StartCoroutine(Teleport(lift));
        }

        if (hit.collider.TryGetComponent<Door>(out Door door))
        {
            StartCoroutine(OpenDoor(door));
        }

        if (hit.collider.TryGetComponent<Bed>(out Bed bed))
        {
            bed.Interact(); // triggers scene change
        }

        if (hit.collider.TryGetComponent<FirstDeath>(out FirstDeath firstDeath))
        {
            StartCoroutine(DieFirst()); // triggers scene change
        }

        if (hit.collider.TryGetComponent<SecondDeath>(out SecondDeath secondDeath))
        {
            StartCoroutine(DieSecond()); // triggers scene change
        }

        if (hit.collider.gameObject.name == "FireBin")
        {
            StartCoroutine(StartFire());
        }
    }

    IEnumerator Teleport(Lift lift)
    {
        player.GetComponent<CharacterController>().enabled = false;

        yield return new WaitForSeconds(0.1f);

        player.localPosition = lift.position;
        player.localEulerAngles = lift.rotation;

        yield return new WaitForSeconds(0.1f);

        player.GetComponent<CharacterController>().enabled = true;
        if (audioSource != null && sleepingClip != null)
        {
            audioSource.clip = sleepingClip;
            audioSource.Play();
        }

    }

    IEnumerator OpenDoor(Door door)
    {

        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(nextScene);

    }

    private IEnumerator DieFirst()
    {

        if (audioSource != null && sleepingClip != null && blackScreen != null)
        {
            blackScreen.SetActive(true);
            audioSource.clip = sleepingClip;
            audioSource.Play();

            // Wait until the clip finishes
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        // Change the scene
        SceneManager.LoadScene("DiedOnce");
    }

    private IEnumerator DieSecond()
    {

        if (audioSource != null && sleepingClip != null && blackScreen != null)
        {
            blackScreen.SetActive(true);
            audioSource.clip = sleepingClip;
            audioSource.Play();

            // Wait until the clip finishes
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        // Change the scene
        SceneManager.LoadScene("DiedTwice");
    }
    IEnumerator StartFire()
    {
        Debug.Log("Fire");
        ps.Play();
        audioSource.clip = eerieClip;
        audioSource.Play();

        // Wait for 7 seconds while particle system and audio play
        yield return new WaitForSeconds(7f);

        // Stop the particle system
        ps.Stop();

        // Activate the object
        firebins.SetActive(true);

        // Wait another 5 seconds before showing texts
        yield return new WaitForSeconds(5f);

        // Fade in first text ("Thank you")
        if (thankYouText != null)
        {
            thankYouText.gameObject.SetActive(true);
            thankYouText.text = "Thank you for Playing!";
            yield return StartCoroutine(FadeInText(thankYouText, 3f));
        }

        // Wait a bit before fading in the second text
        yield return new WaitForSeconds(1.5f);

        // Fade in second text ("Your free" or whatever)
        if (yourFree != null)
        {
            yourFree.gameObject.SetActive(true);
            yourFree.text = "You are free";
            yield return StartCoroutine(FadeInText(yourFree, 3f));
        }
    }

    IEnumerator FadeInText(TMP_Text text, float duration)
    {
        float elapsed = 0f;
        Color c = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / duration); // Lerp alpha from 0 → 1
            text.color = c;
            yield return null;
        }

        c.a = 1;
        text.color = c; // ensure fully visible at the end
    }

}
