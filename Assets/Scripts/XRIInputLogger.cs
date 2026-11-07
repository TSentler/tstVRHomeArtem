
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class XRIInputLogger : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogError("Не назначен Input Action Asset!");
            return;
        }

        inputActions.Enable();
    } 

    private void Update()
    {
        if (inputActions == null) return;

        foreach (var map in inputActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                if (!action.enabled) continue;

                foreach (var control in action.controls)
                {
                    if (control is ButtonControl button &&
                        button.wasPressedThisFrame)
                    {
                        Debug.Log(
                            $"Action: {action.name} | " +
                            $"Map: {map.name} | " +
                            $"Control: {control.path}"
                        );
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
            inputActions.Disable();
    }
}
