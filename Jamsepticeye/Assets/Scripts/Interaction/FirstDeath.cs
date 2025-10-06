using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstDeath : MonoBehaviour
{
    // Start is called before the first frame update
    public void Interact()
    {
        /*        if (hasBeenUsed) return; // only allow once
                hasBeenUsed = true;*/

        // Optional: play sound, animation, fade, etc.
        Debug.Log("Bed used! Loading next scene...");

        // Load the next scene
        SceneManager.LoadScene("DiedOnce");
    }
}
