using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class DartStick : MonoBehaviour
{
    [Header("—сылки")]
    [Tooltip("ѕустышка на самом кончике иглы")]
    public Transform tip;

    [Header(" уда можно втыкатьс€")]
    public LayerMask stickLayers = ~0;

    [Header("”слови€ втыкани€")]
    [Tooltip("ћедленнее Ч просто отскочит")]
    public float minSpeed = 1.5f;
    [Tooltip("ћакс. угол между иглой и направлением полЄта")]
    public float maxAngle = 40f;

    [Header("—пособ креплени€")]
    [Tooltip("ON = FixedJoint, OFF = kinematic + parent (стабильнее)")]
    public bool useFixedJoint = false;

    private Rigidbody rb;
    private XRGrabInteractable grab;
    private FixedJoint joint;
    private Collider[] myColliders;

    private bool isStuck;
    private Vector3 lastVelocity;
    private Transform originalParent;

    // с кем отключили коллизии, чтобы потом вернуть
    private readonly List<Collider> ignoredColliders = new List<Collider>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
        myColliders = GetComponentsInChildren<Collider>();
        originalParent = transform.parent;

        if (tip == null) tip = transform;

        // критично дл€ быстрых бросков Ч иначе дротик пролетит сквозь доску
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnEnable() => grab.selectEntered.AddListener(OnGrabbed);
    void OnDisable() => grab.selectEntered.RemoveListener(OnGrabbed);

    void FixedUpdate()
    {
        // запоминаем скорость ƒќ удара Ч в OnCollisionEnter она уже погашена
        if (!isStuck && !rb.isKinematic)
            lastVelocity = rb.linearVelocity;   // Unity 2022 и старше: rb.velocity
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isStuck) return;
        if (grab.isSelected) return;                       // в руке Ч не втыкаем
        if (((1 << collision.gameObject.layer) & stickLayers) == 0) return;

        if (lastVelocity.magnitude < minSpeed) return;

        // летим ли иглой вперЄд, а не боком
        Vector3 needleDir = (tip.position - transform.position).normalized;
        if (needleDir == Vector3.zero) needleDir = transform.forward;
        if (Vector3.Angle(needleDir, lastVelocity) > maxAngle) return;

        Stick(collision);
    }

    void Stick(Collision collision)
    {
        isStuck = true;

        // подт€нуть дротик так, чтобы кончик иглы лЄг ровно в точку удара
        ContactPoint contact = collision.GetContact(0);
        transform.position += contact.point - tip.position;

        rb.linearVelocity = Vector3.zero;                  // Unity 2022: rb.velocity
        rb.angularVelocity = Vector3.zero;

        // чтобы коллайдеры не толкали дротик обратно
        foreach (Collider mine in myColliders)
        {
            foreach (Collider theirs in collision.gameObject.GetComponentsInChildren<Collider>())
            {
                if (theirs == null || theirs.isTrigger) continue;
                Physics.IgnoreCollision(mine, theirs, true);
                ignoredColliders.Add(theirs);
            }
        }

        if (useFixedJoint)
        {
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;     // null = прибить к миру
            joint.enableCollision = false;
            joint.breakForce = Mathf.Infinity;
        }
        else
        {
            rb.isKinematic = true;
            transform.SetParent(collision.transform, true);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        Unstick();
    }

    void Unstick()
    {
        if (!isStuck) return;
        isStuck = false;

        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }

        transform.SetParent(originalParent, true);
        rb.isKinematic = false;

        foreach (Collider mine in myColliders)
        {
            foreach (Collider theirs in ignoredColliders)
            {
                if (mine != null && theirs != null)
                    Physics.IgnoreCollision(mine, theirs, false);
            }
        }
        ignoredColliders.Clear();
    }
}
