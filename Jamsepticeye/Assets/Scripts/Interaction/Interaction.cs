using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    Transform player;
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
    } 
}
