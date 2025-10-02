using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectName = "Object"; // For debugging or UI prompts

    // This gets called when player interacts
    public virtual void Interact()
    {
        Debug.Log("Interacted with: " + objectName);
        // Override this in child scripts for custom behavior
    }
}
