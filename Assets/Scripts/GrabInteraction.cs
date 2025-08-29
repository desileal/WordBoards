using UnityEngine;

public class GrabInteraction : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            OnGrabReleased();
        }
    }

    public void OnGrabReleased()
    {
        Debug.Log("Released cube");

    }
}
