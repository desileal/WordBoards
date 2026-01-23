using Oculus.Haptics;
using Oculus.Interaction;
using UnityEngine;

public class HapticOnGrab : MonoBehaviour
{
    public HapticClip hapticClip; // Optional: Predefined haptic pattern

    /**
    private void Awake()
    {
        var grabbable = GetComponent<Grabbable>();
        grabbable.WhenGrabbing += () => TriggerHaptics(grabbable.GrabInteractor);
    }

    private void TriggerHaptics(IInteractor interactor)
    {
        if (interactor is ControllerInteractor controller)
        {
            OVRInput.Controller ovrController = controller.Controller;
            OVRInput.SetControllerVibration(1f, 0.5f, ovrController); // (frequency, amplitude, controller)
            Invoke(nameof(StopVibration), 0.1f);
        }
    }

    private void StopVibration()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
    **/
}
