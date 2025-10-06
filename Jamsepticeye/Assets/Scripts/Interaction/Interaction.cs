using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class Interaction : MonoBehaviour
{
    Transform player;

    public AudioSource audioSource;

    public AudioClip sleepingClip;

    public GameObject blackScreen;

    public string nextScene;

    public ParticleSystem ps;
    public GameObject firebins;
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

        // Wait for 7 seconds
        yield return new WaitForSeconds(7f);

        // Stop the particle system
        ps.Stop();

        // Activate the object
        firebins.SetActive(true);
    }
}
