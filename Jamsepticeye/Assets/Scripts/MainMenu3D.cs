using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class MainMenu3D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Assign the child that VISUALLY scales (not the collider parent)")]
    public Transform visual;                    // ← drag your mesh/renderer child here

    [Header("Scale")]
    public float hoverMultiplier = 1.08f;       // 8% bigger on hover
    public float smoothTime = 0.08f;            // smoothing time (lower = snappier)

    private Vector3 baseScale;
    private Vector3 targetScale;
    private Vector3 scaleVelocity;              // for SmoothDamp (vector overload)

    public UnityEvent OnClick;

    private bool hovering;

    void Awake()
    {
        if (visual == null) visual = transform; // fallback
        baseScale = visual.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        targetScale = hovering ? baseScale * hoverMultiplier : baseScale;

        // SmoothDamp toward target to avoid jitter/overshoot
        visual.localScale = new Vector3(
            Mathf.SmoothDamp(visual.localScale.x, targetScale.x, ref scaleVelocity.x, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime),
            Mathf.SmoothDamp(visual.localScale.y, targetScale.y, ref scaleVelocity.y, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime),
            Mathf.SmoothDamp(visual.localScale.z, targetScale.z, ref scaleVelocity.z, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime)
        );
    }

    public void OnPointerEnter(PointerEventData e) => hovering = true;
    public void OnPointerExit(PointerEventData e) => hovering = false;

    public void OnPointerClick(PointerEventData e)
    {
        OnClick?.Invoke();
    }
}
