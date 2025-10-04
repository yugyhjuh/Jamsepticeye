using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform objectGrabPointTransform;
    private bool isGrabbed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        rb.useGravity = false;
        //rb.drag = 10f; // optional: smooth movement while grabbed
        isGrabbed = true;
    }

    public void Drop()
    {
        objectGrabPointTransform = null;
        rb.useGravity = true;
       // rb.drag = 0f; // reset drag so physics is normal
        isGrabbed = false;
    }

    private void FixedUpdate()
    {
        if (isGrabbed && objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.fixedDeltaTime * lerpSpeed);
            rb.MovePosition(newPosition);
        }
    }
}
