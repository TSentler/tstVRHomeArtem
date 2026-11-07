using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class VRInputLogger : MonoBehaviour
{
    void Update()
    {
        foreach (var device in InputSystem.devices)
        {
            foreach (var control in device.allControls)
            {
                if (control is ButtonControl button &&
                    button.wasPressedThisFrame)
                {
                    Debug.Log(
                        $"{device.displayName} -> {control.path}"
                    );
                }
            }
        }
    }
}
