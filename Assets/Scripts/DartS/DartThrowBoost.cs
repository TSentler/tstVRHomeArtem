using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class DartThrowBoost : MonoBehaviour
{
    [Header("Усиление броска")]
    [Tooltip("Во сколько раз умножить скорость в момент отпускания")]
    public float throwMultiplier = 2.5f;

    [Tooltip("Ниже этой скорости бросок не усиливаем — это просто выпустили из руки")]
    public float minThrowSpeed = 0.8f;

    [Tooltip("Потолок, чтобы дротик не улетал в космос от резкого рывка")]
    public float maxSpeed = 14f;

    [Header("Ускорение в полёте")]
    [Tooltip("Доп. ускорение вдоль направления полёта, м/с²")]
    public float flightAcceleration = 6f;

    [Tooltip("Сколько секунд после броска действует ускорение")]
    public float accelerationTime = 0.35f;

    [Header("Компенсация гравитации")]
    [Range(0f, 1f)]
    [Tooltip("1 = дротик летит почти по прямой, 0 = обычная дуга")]
    public float gravityCompensation = 0.5f;

    private Rigidbody rb;
    private XRGrabInteractable grab;

    private bool isBoosting;
    private float boostEndTime;
    private Vector3 boostDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grab.selectExited.AddListener(OnReleased);
        grab.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        grab.selectExited.RemoveListener(OnReleased);
        grab.selectEntered.RemoveListener(OnGrabbed);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        isBoosting = false;   // взяли в руку посреди полёта — глушим ускорение
    }

    void OnReleased(SelectExitEventArgs args)
    {
        // XRI сам придаёт скорость при отпускании, ждём один FixedUpdate
        // и только потом умножаем — иначе перезапишем его же значение
        Invoke(nameof(ApplyBoost), 0f);
    }

    void ApplyBoost()
    {
        Vector3 v = rb.linearVelocity;   // Unity 2022 и старше: rb.velocity
        Debug.Log(v.magnitude);
        if (v.magnitude < minThrowSpeed) return;

        v *= throwMultiplier;
        v = Vector3.ClampMagnitude(v, maxSpeed);
        rb.linearVelocity = v;           // Unity 2022: rb.velocity

        boostDirection = v.normalized;
        boostEndTime = Time.time + accelerationTime;
        isBoosting = true;
    }

    void FixedUpdate()
    {
        if (!isBoosting) return;

        if (Time.time > boostEndTime || grab.isSelected)
        {
            isBoosting = false;
            return;
        }

        // разгон вдоль направления броска
        rb.AddForce(boostDirection * flightAcceleration, ForceMode.Acceleration);

        // частично гасим падение, чтобы траектория была более настильной
        if (gravityCompensation > 0f)
            rb.AddForce(-Physics.gravity * gravityCompensation, ForceMode.Acceleration);
    }
}