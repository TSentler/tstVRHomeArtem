using UnityEngine;

/// <summary>
/// Вешается на оружие (объект с XR Grab Interactable).
/// Метод Fire() вызывается из события Activated (нажатие триггера) в инспекторе.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BulletSpawner : MonoBehaviour
{
    [Header("Пуля")]
    [SerializeField] private GameObject bulletPrefab;   // префаб с Rigidbody + Bullet
    [SerializeField] private Transform muzzle;          // пустышка на конце ствола (ось Z смотрит вперёд)
    [SerializeField] private float bulletSpeed = 40f;

    [Header("Эффекты")]
    [SerializeField] private GameObject muzzleFlashPrefab; // префаб с ParticleSystem
    [SerializeField] private float muzzleFlashLifetime = 2f;

    [Header("Звук")]
    [SerializeField] private AudioClip[] shotSounds;       // можно несколько для разнообразия
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Стрельба")]
    [SerializeField] private float fireCooldown = 0.15f;   // защита от спама триггером

    private AudioSource audioSource;
    private float nextFireTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D-звук — в VR это важно
    }

    /// <summary>Привязать к Activated у XR Grab Interactable.</summary>
    public void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireCooldown;

        SpawnBullet();
        SpawnMuzzleFlash();
        PlayShotSound();
    }

    private void SpawnBullet()
    {
        if (bulletPrefab == null || muzzle == null) return;

        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);

        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            // Быстрая пуля иначе пролетит сквозь тонкие коллайдеры
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = muzzle.forward * bulletSpeed; // в Unity 2022 и старше: rb.velocity
        }
    }

    private void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzle == null) return;

        GameObject fx = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation);
        Destroy(fx, muzzleFlashLifetime);
    }

    private void PlayShotSound()
    {
        if (shotSounds == null || shotSounds.Length == 0) return;

        AudioClip clip = shotSounds[Random.Range(0, shotSounds.Length)];
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(clip, volume);
    }
}
