using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Ledge : MonoBehaviour
{

    public Transform snapPoint;

    [SerializeField]
    public string targetID;


    [Header("Optional Acceptance Volume")]
    [Tooltip("If assigned, cube must be inside this trigger after grab release.")]
    public Collider acceptanceBoundary; // should be a Trigger


    public bool IsInsideBoundary(Bounds cubeBounds)
    {
        if (acceptanceBoundary == null) return true; // no boundary means always valid
        // Use bounds check for robustness (vs. just a point)
        return acceptanceBoundary.bounds.Intersects(cubeBounds);
    }
}

