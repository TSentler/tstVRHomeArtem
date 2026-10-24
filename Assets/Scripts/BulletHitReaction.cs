using UnityEngine;

/// <summary>
/// Меняет анимацию (через триггер Animator) при попадании пули.
/// Повесить на объект с Collider/Collider2D и Animator.
/// </summary>
[RequireComponent(typeof(Animator))]
public class BulletHitReaction : MonoBehaviour
{
    [Header("Кто считается пулей")]
    [SerializeField] private string bulletTag = "Bullet";

    [Header("Animator")]
    [SerializeField] private string hitTrigger = "Hit";   // имя Trigger-параметра в Animator
    [SerializeField] private float hitCooldown = 0.3f;    // защита от спама попаданиями

    [Header("Пуля")]
    [SerializeField] private bool destroyBullet = true;

    private Animator animator;
    private int hitHash;
    private float lastHitTime = -999f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hitHash = Animator.StringToHash(hitTrigger);
    }

    // ---- 3D (не-триггерный Collider + Rigidbody у одного из объектов) ----
    private void OnCollisionEnter(Collision collision)
    {
        TryReact(collision.gameObject);
    }

    // ---- 3D, если пуля — Trigger ----
    private void OnTriggerEnter(Collider other)
    {
        TryReact(other.gameObject);
    }

    // ---- 2D ----
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryReact(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryReact(other.gameObject);
    }

    private void TryReact(GameObject other)
    {
        if (!other.CompareTag(bulletTag)) return;
        if (Time.time - lastHitTime < hitCooldown) return;

        lastHitTime = Time.time;
        animator.SetTrigger(hitHash);

        if (destroyBullet)
            Destroy(other);
    }
}
