using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform grabPoint;
    private bool isGrabbed;

    [SerializeField] private float followSpeed = 15f; // how quickly it follows the grab point
    [SerializeField] private float dropForce = 8f;     // downward impulse when dropped

private void Awake()
{
    rb = GetComponent<Rigidbody>();
    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Prevent clipping
}

    public void Grab(Transform grabPoint)
    {
        this.grabPoint = grabPoint;
        isGrabbed = true;
        rb.useGravity = false;
        rb.drag = 10f;              // high drag while held (less jitter)
        rb.velocity = Vector3.zero; // reset momentum
    }

    public void Drop()
    {
        isGrabbed = false;
        grabPoint = null;
        rb.useGravity = true;
        rb.drag = 0f;   // restore normal physics

        // Add a downward impulse to make it fall faster
        rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (isGrabbed && grabPoint != null)
        {
            Vector3 direction = grabPoint.position - rb.position;
            float distance = direction.magnitude;

            // Scale force by distance to prevent overshooting
            float followForce = 50f; // tweak this
            rb.AddForce(direction.normalized * followForce * distance, ForceMode.Force);
        }
    }
}
