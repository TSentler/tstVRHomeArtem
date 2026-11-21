using UnityEngine;
using System.Collections.Generic;

public class XRLocomotionSwitcher : MonoBehaviour
{
    [Header("Компоненты, которые нужно включать/выключать")]
    public List<Behaviour> componentsToToggle = new List<Behaviour>();

    /// <summary>
    /// Выключить все компоненты из списка
    /// </summary>
    public void DisableAll()
    {
        for (int i = componentsToToggle.Count-1; i >= 0; i--)
        {
            var comp = componentsToToggle[i];
            if (comp != null)
                comp.enabled = false;
        }
    }

    /// <summary>
    /// Включить все компоненты из списка
    /// </summary>
    public void EnableAll()
    {
        foreach (var comp in componentsToToggle)
        {
            if (comp != null)
                comp.enabled = true;
        }
    }
}