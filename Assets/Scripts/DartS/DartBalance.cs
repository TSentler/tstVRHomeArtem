using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DartBalance : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Та же пустышка на острие, что и в DartStick")]
    public Transform tip;

    [Header("Центр масс")]
    [Tooltip("0 = центр модели, 1 = ровно в игле")]
    [Range(0f, 1f)]
    public float massShiftToTip = 0.7f;

    [Tooltip("Показывать центр масс в сцене")]
    public bool drawGizmo = true;

    [Header("Доворот иглой вперёд (аэродинамика)")]
    [Tooltip("Имитация оперения — доворачивает дротик по направлению полёта")]
    public bool alignToVelocity = true;

    [Tooltip("Сила доворота. Больше — резче выравнивается")]
    public float alignStrength = 8f;

    [Tooltip("Ниже этой скорости не доворачиваем — дротик почти завис")]
    public float minAlignSpeed = 1.0f;

    private Rigidbody rb;
    private Vector3 localCenter;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ApplyCenterOfMass();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;
        if (rb == null) rb = GetComponent<Rigidbody>();
        ApplyCenterOfMass();
    }

    public void ApplyCenterOfMass()
    {
        if (tip == null) return;

        // точка острия в локальных координатах дротика
        Vector3 tipLocal = transform.InverseTransformPoint(tip.position);

        // сдвигаем от геометрического центра к игле
        localCenter = Vector3.Lerp(Vector3.zero, tipLocal, massShiftToTip);
        rb.centerOfMass = localCenter;
    }

    void FixedUpdate()
    {
        if (!alignToVelocity) return;
        if (rb.isKinematic) return;

        Vector3 v = rb.linearVelocity;   // Unity 2022 и старше: rb.velocity
        if (v.magnitude < minAlignSpeed) return;

        // ось иглы
        Vector3 needleDir = (tip != null)
            ? (tip.position - transform.position).normalized
            : transform.forward;

        if (needleDir == Vector3.zero) return;

        // момент, доворачивающий иглу по вектору скорости
        Vector3 torque = Vector3.Cross(needleDir, v.normalized);
        rb.AddTorque(torque * alignStrength, ForceMode.Acceleration);
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmo) return;

        Rigidbody body = GetComponent<Rigidbody>();
        if (body == null || tip == null) return;

        Vector3 tipLocal = transform.InverseTransformPoint(tip.position);
        Vector3 com = Vector3.Lerp(Vector3.zero, tipLocal, massShiftToTip);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.TransformPoint(com), 0.012f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, tip.position);
    }
}