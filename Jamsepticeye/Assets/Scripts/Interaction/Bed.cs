using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour
{
    public string nextSceneName = "HouseFirstDeath"; // name of the scene to load
    //private bool hasBeenUsed = false;

    public void Interact()
    {
/*        if (hasBeenUsed) return; // only allow once
        hasBeenUsed = true;*/

        // Optional: play sound, animation, fade, etc.
        Debug.Log("Bed used! Loading next scene...");

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
