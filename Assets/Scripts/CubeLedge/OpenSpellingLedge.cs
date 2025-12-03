using System.Collections.Generic;
using UnityEngine;

// TODO: 
// 
public class OpenSpellingLedge : MonoBehaviour
{
    public Transform snapPoint;
     
    [Header("Layout")]
    [SerializeField] private float blockOffset = 0.2f;   // Distance between block centers
    [SerializeField] private float yOffset = 0.05f;      // Height relative to ledge
    [SerializeField] private float zOffset = 0f;      // Forward offset relative to ledge

    public List<Transform> _blocksOnLedge = new List<Transform>();

    [Header("Optional Acceptance Volume")]
    [Tooltip("If assigned, cube must be inside this trigger after grab release.")]
    public Collider acceptanceBoundary; // should be a Trigger

    CentralEventSystem CES;

    private void Awake()
    {
        CES = CentralEventSystem.Instance;

        if (CES)
        {
            CES.OnAdjustLedgeTransformPositions += AddBlock;
        }
    }

    public bool IsInsideBoundary(Bounds cubeBounds)
    {
        if (acceptanceBoundary == null) return true; 
        return acceptanceBoundary.bounds.Intersects(cubeBounds);
    }

    /// <summary>
    /// Called when a letter has been placed on the ledge.
    /// </summary>
    public void AddBlock(Transform t)
    {
        if(_blocksOnLedge.Count == 0)
        {
            t.position = snapPoint.position;
            t.rotation = snapPoint.rotation;
            _blocksOnLedge.Add(t);
            return;
        }
        
        t.position = _blocksOnLedge[0].position;
        t.rotation = _blocksOnLedge[0].rotation;
        //_blocksOnLedge.Add(snapPoint);

        RepositionBlocks();
    }

    // TODO: remove a tra
    /// <summary>
    /// Called when a letter is removed from the ledge (e.g., grabbed again).
    /// </summary>
    public void RemoveBlock(int i)
    {
        if(_blocksOnLedge.Count <= 0) return;

        _blocksOnLedge.RemoveAt(i);
        RepositionBlocks();
    }

    /// <summary>
    /// Recompute local X positions for all blocks on the ledge.
    /// </summary>
    private void RepositionBlocks()
    {
        int count = _blocksOnLedge.Count;
        if (count == 0) return;

        // Center blocks: total span based on spacing and count
        float totalWidth = (count - 1) * blockOffset;
        float leftEdge = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform block = _blocksOnLedge[i];

            float x = leftEdge + i * blockOffset;
            var targetLocalPos = new Vector3(x, yOffset, zOffset);

            Debug.LogWarning($"Repositioning block at index {i} to {targetLocalPos.ToString()}"); 

            block.localPosition = targetLocalPos;
            block.localRotation = gameObject.transform.rotation; 
        }
        CES?.InvokeOnUpdateLedgeLetterTransforms();
    }

}
