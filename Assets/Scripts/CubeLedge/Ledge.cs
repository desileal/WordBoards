using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Ledge : MonoBehaviour
{
    public Transform snapPoint;

    public int letterIndex;
    public bool snapped = false;

    [Header("Optional Acceptance Volume")]
    [Tooltip("If assigned, cube must be inside this trigger after grab release.")]
    public Collider acceptanceBoundary; // should be a Trigger

    CentralEventSystem CES;

    private void Awake()
    {
        CES = CentralEventSystem.Instance;
    }

    public bool IsInsideBoundary(Bounds cubeBounds)
    {
        if (acceptanceBoundary == null) return true; // no boundary means always valid
        // Use bounds check for robustness (vs. just a point)
        return acceptanceBoundary.bounds.Intersects(cubeBounds);
    }

    public void OnTriggerEnter(Collider other)
    {
        // if the ledge already has a block snapped to it, ignore other colliders
        if (snapped) return;

        if (other.CompareTag("Cube"))
        {
            Debug.Log($"Collision detected with ledge at letterIndex {letterIndex}");
            CES.InvokeOnLedgeCollision(letterIndex);
        }
    }
}

