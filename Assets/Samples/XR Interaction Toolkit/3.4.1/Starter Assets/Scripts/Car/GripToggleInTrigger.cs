using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GripToggleInTrigger : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference gripAction;   // Правый или левый Grip

    [Header("Actions")]
    public UnityEvent onFirstPress;          // Первое нажатие (например: сесть в машину)
    public UnityEvent onSecondPress;         // Второе нажатие (например: выйти из машины)

    [Header("Settings")]
    public bool resetOnExitTrigger = true;   // Сбрасывать состояние при выходе из зоны

    private bool isInsideTrigger = false;
    private bool isFirstState = true;        // true = ждём первое нажатие

    void OnEnable()
    {
        if (gripAction != null)
        {
            gripAction.action.performed += OnGripPressed;
            gripAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (gripAction != null)
        {
            gripAction.action.performed -= OnGripPressed;
            gripAction.action.Disable();
        }
    }

    private void OnGripPressed(InputAction.CallbackContext context)
    {
        if (!isInsideTrigger) return;

        if (isFirstState)
        {
            onFirstPress?.Invoke();
            isFirstState = false;
        }
        else
        {
            onSecondPress?.Invoke();
            isFirstState = true;
        }
    }

    // === Триггер зона ===
    private void OnTriggerEnter(Collider other)
    {
        // Можно добавить проверку тега, например: if (!other.CompareTag("Player")) return;
        isInsideTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInsideTrigger = false;

        if (resetOnExitTrigger)
        {
            isFirstState = true; // Сбрасываем, чтобы при следующем входе снова было первое действие
        }
    }
}