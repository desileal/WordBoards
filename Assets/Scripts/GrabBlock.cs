using UnityEngine;

public class GrabBlock : CubeInteraction
{

    private bool ledgeCollision = false;
    private int _snappedLedgeIndex = -1;

    public GrabBlock(string id, int index) : base(id, index)
    {
        targetID = id;
        listIndex = index;
    }

    /// <summary>
    /// Handles collisions with the cube. Registers poke interactions and handles selection logic from there.
    /// </summary>
    /// <param name="other">The Collider that has collided with the cube collider </param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Grab block detected a collision with another object.");
        if (_isSnapping) return;

        // check of other collider is a ledge and set a flag to that
        if (other.CompareTag("Ledge"))
        {
            Debug.Log($"Grab block of letter of {targetID} collided with ledge.");
            ledgeCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // check of other collider is a ledge and set a flag to that
        if (other.CompareTag("Ledge"))
        {
            Debug.Log("Grab block exited ledge collider.");
            ledgeCollision = false;
        }
    }

    public void SetSnappedLedgeIndex(int ledgeIndex)
    {
        _snappedLedgeIndex = ledgeIndex;
    }

    public void ResetSnappedLedgeIndex()
    {
        _snappedLedgeIndex = -1;
    }

    public void OnGrabSelected()
    {
        Debug.Log($"Grabbed block of letter {targetID}.");
        _isHeld = true;

        if (_snappedLedgeIndex != -1)
        {
            CES.InvokeOnBlockRemovedFromLedge(_snappedLedgeIndex);
            _snappedLedgeIndex = -1;
        }
    }

    /// <summary>
    /// Called by grab system when the cube is released (When Unselect())
    /// </summary>
    public void OnGrabReleased()
    {
        Debug.Log($"Released grab block of letter {targetID}");

        // check to see where cube location is - if it triggered collision with the proper slot

        _isHeld = false;
        if (_isSnapping)
        {
            Debug.LogWarning($"[GRAB RELEASED] Aborting because _isSnapping=true!");
            return;
        }

        // var cubeBounds = ComputeWorldBounds();
        if (ledgeCollision) // previously checked targetLedge.IsInsideBoundary(cubeBounds)
        {
            Debug.Log($"[GRAB RELEASED] Invoking OnPlayerGrabRelease");
            CES.InvokeOnPlayerGrabRelease(targetID, listIndex);
        }
        else
        {
            Debug.Log($"[GRAB RELEASED] No ledge collision, returning to start");
            Debug.Log($"Returning grab block {targetID} to starting position");
            ReturnToStartPosition();
        }
    }


    public void ReturnToStartPosition()
    {
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(_startPos, _startRot, returnDuration));
    }

}
