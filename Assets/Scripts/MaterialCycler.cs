using System.Collections.Generic;
using UnityEngine;

public class MaterialCycler : MonoBehaviour
{
    public List<Material> materials;  // Assign in Inspector
    private MeshRenderer meshRenderer;
    private int currentIndex = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Optional: apply the first material at startup
        if (materials.Count > 0)
        {
            meshRenderer.material = materials[0];
        }
    }

    public void CycleMaterial()
    {
        if (materials.Count == 0) return;

        currentIndex = (currentIndex + 1) % materials.Count;
        meshRenderer.material = materials[currentIndex];
        Debug.Log($"Changing cube material after being poked to {meshRenderer.material}");
    }
}
