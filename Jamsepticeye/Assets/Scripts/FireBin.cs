using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBin : MonoBehaviour
{
    public ParticleSystem particles; // Assign in Inspector

    public void Interact()
    {
        if (particles != null)
        {
            particles.Stop();                 // stops emission immediately
            particles.gameObject.SetActive(false); // hides the system completely
        }
    }
}
