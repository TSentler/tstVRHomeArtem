using UnityEngine;
using UnityEngine.InputSystem;

public class GrokSimpleVRCarController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Input Actions")]
    public InputActionReference turnStickAction;   // Левый стик (поворот)
    public InputActionReference throttleStickAction;  // Правый стик (газ / задний ход)

    [Header("Movement")]
    public float maxSpeed = 25f;
    public float acceleration = 35f;
    public float reverseAcceleration = 18f;     // Задний ход слабее — важно!
    public float turnSpeed = 80f;
    public float drag = 2.2f;

    [Header("Stability")]
    public float centerOfMassY = -0.6f;         // Чем ниже — тем стабильнее (пробуй -0.5 ... -0.9)

    private Vector2 turnStick;
    private Vector2 throttleStick;

    void Start()
    {
        // Опускаем центр масс — это самое важное против "дыбов"
        rb.centerOfMass = new Vector3(0f, centerOfMassY, 0f);

    }

    void FixedUpdate()
    {
        turnStick = turnStickAction?.action.ReadValue<Vector2>() ?? Vector2.zero;
        throttleStick = throttleStickAction?.action.ReadValue<Vector2>() ?? Vector2.zero;

        // === Поворот ===
        float steer = turnStick.x;
        transform.Rotate(0f, steer * turnSpeed * Time.fixedDeltaTime, 0f);

        // === Газ / Задний ход ===
        float throttle = throttleStick.y;

        // Мёртвая зона
        if (Mathf.Abs(throttle) < 0.12f) throttle = 0f;

        if (throttle != 0f)
        {
            // Берём только горизонтальное направление вперёд/назад
            Vector3 moveDir = transform.forward;
            moveDir.y = 0f;
            moveDir.Normalize();

            float force = throttle > 0
                ? throttle * acceleration
                : throttle * reverseAcceleration;

            rb.AddForce(moveDir * force, ForceMode.Acceleration);
        }

        // Ограничение скорости
        Vector3 horizontalVel = rb.linearVelocity;
        horizontalVel.y = 0f;

        if (horizontalVel.magnitude > maxSpeed)
        {
            horizontalVel = horizontalVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);
        }

        // Drag только по горизонтали
        Vector3 vel = rb.linearVelocity;
        vel.x *= (1f - drag * Time.fixedDeltaTime);
        vel.z *= (1f - drag * Time.fixedDeltaTime);
        rb.linearVelocity = vel;
    }
}