using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

public class ResetHeight : MonoBehaviour
{
    public InputActionProperty aButtonAction; // Reference to the Input Action

    void Update()
    {
        if (aButtonAction.action.WasPressedThisFrame())
        {
            Debug.Log("A Button Pressed!");
            ResetOriginHeight();
        }
    }

    void ResetOriginHeight()
    {
        XRInputSubsystem xrInput = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();

        if (xrInput != null)
        {
            xrInput.TryRecenter();
            Debug.Log("XR Origin recentered.");
        }
    }
}