using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class DartStickSimple2 : MonoBehaviour
{
    public float minSpeed = 1.5f;

    public Rigidbody rb;
    public XRGrabInteractable grab;
    public bool isStuck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbing);   // ƒќ того, как XRI снимет слепок
        grab.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbing);
        grab.selectExited.RemoveListener(OnReleased);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isStuck) return;
        if (grab.isSelected) return;
        if (collision.relativeVelocity.magnitude < minSpeed) return;

        isStuck = true;
        rb.isKinematic = true;
        transform.SetParent(collision.transform);
    }

    void OnGrabbing(SelectEnterEventArgs args)
    {
        if (!isStuck) return;

        isStuck = false;
        transform.SetParent(null);
        rb.isKinematic = false;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        // XRI мог восстановить кинематику из слепка Ч снимаем
        rb.isKinematic = false;
    }
}