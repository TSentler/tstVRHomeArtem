using UnityEngine;

/// <summary>
/// Вешается на префаб пули (нужны Rigidbody и Collider).
/// При попадании спавнит эффект, проигрывает звук и удаляется.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;           // самоуничтожение, если никуда не попала
    [SerializeField] private GameObject hitEffectPrefab;    // искры / дым / пыль
    [SerializeField] private float hitEffectLifetime = 2f;
    [SerializeField] private AudioClip hitSound;
    [SerializeField, Range(0f, 1f)] private float hitVolume = 1f;

    private bool hasHit;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return; // чтобы не сработало дважды за кадр
        hasHit = true;

        ContactPoint contact = collision.GetContact(0);

        if (hitEffectPrefab != null)
        {
            // Эффект ориентируем по нормали поверхности
            Quaternion rot = Quaternion.LookRotation(contact.normal);
            GameObject fx = Instantiate(hitEffectPrefab, contact.point, rot);
            Destroy(fx, hitEffectLifetime);
        }

        if (hitSound != null)
        {
            // Звук играет в точке попадания, даже когда пуля уже уничтожена
            AudioSource.PlayClipAtPoint(hitSound, contact.point, hitVolume);
        }

        Destroy(gameObject);
    }
}
