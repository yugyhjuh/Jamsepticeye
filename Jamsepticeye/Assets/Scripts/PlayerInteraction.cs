using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactMask;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform grabPointTransform;

    private Interactable heldObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryGrab();
            }
            else
            {
                DropObject();
            }
        }

        // Update grab point to avoid walls every frame
        UpdateGrabPoint();
    }

    private void TryGrab()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, interactDistance, interactMask))
        {
            if (hit.transform.TryGetComponent(out Interactable interactable))
            {
                heldObject = interactable;
                heldObject.Grab(grabPointTransform);
                Debug.Log("Grabbed: " + hit.transform.name);
            }
        }
        else
        {
            Debug.Log("No object hit within range.");
        }
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.Drop();
            heldObject = null;
            Debug.Log("Dropped object");
        }
    }

    private void UpdateGrabPoint()
    {
        float maxDistance = interactDistance;

        Vector3 origin = playerCameraTransform.position;
        Vector3 forward = playerCameraTransform.forward;

        // Cast from camera to max distance
        if (Physics.Raycast(origin, forward, out RaycastHit hit, maxDistance))
        {
            // Stop grab point before obstacle
            grabPointTransform.position = hit.point - forward * 0.2f; // small buffer
        }
        else
        {
            // Default position in front of player
            grabPointTransform.position = origin + forward * maxDistance;
        }

        // Ensure grab point doesn't go inside the player/camera
        float minDistance = 50f; // minimum distance from camera
        float distance = Vector3.Distance(origin, grabPointTransform.position);
        if (distance < minDistance)
        {
            grabPointTransform.position = origin + forward * minDistance;
        }
    }
}