using UnityEngine;

public class VRCarSeat : MonoBehaviour
{
    public Transform driverSeat;   // пустой объект внутри машины (место водителя)
    public Transform xrOrigin;     // твой XR Origin (или Camera Offset)

    public void EnterCar()
    {
        if (driverSeat != null && xrOrigin != null)
        {
            // Перемещаем игрока в машину
            xrOrigin.SetParent(driverSeat);
            xrOrigin.localPosition = Vector3.zero;
            xrOrigin.localRotation = Quaternion.identity;
        }
    }

    // Если нужно высадить игрока
    public void ExitCar()
    {
        if (xrOrigin != null)
        {
            xrOrigin.SetParent(null);
            xrOrigin.localPosition += transform.right;
        }
    }
}