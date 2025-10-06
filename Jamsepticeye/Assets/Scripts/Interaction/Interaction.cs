using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    Transform player;

    public AudioSource audioSource;

    public AudioClip sleepingClip;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
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
            firstDeath.Interact(); // triggers scene change
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

        SceneManager.LoadScene("House");

    }
}
