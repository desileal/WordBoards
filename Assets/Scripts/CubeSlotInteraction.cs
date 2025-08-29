using UnityEngine;

public class CubeSlotInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            //other.SnapToLedge
        }
    }
}
