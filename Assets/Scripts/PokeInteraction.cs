using UnityEngine;

public class PokeInteraction : MonoBehaviour
{
    [SerializeField]
    MaterialCycler materialCycler;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody sourceRigidbody = other.attachedRigidbody;

        if (sourceRigidbody == null) return;

        GameObject sourceObject = sourceRigidbody.gameObject;

        if(other.CompareTag("InputController") || (sourceObject.name.Contains("Hand") && sourceObject.name.Contains("Rigidbody"))) {
            OnPoke(sourceObject.name);
        }
    }

    public void OnPoke(string name)
    {
        Debug.Log($"Poked cube using {name}");

        materialCycler.CycleMaterial();

        //AddCubeToLedge();
        // update sentence
    }
}
