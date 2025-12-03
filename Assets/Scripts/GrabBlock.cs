using UnityEngine;

public class GrabBlock : CubeInteraction
{

    private bool ledgeCollision = false;
    private bool openSpellingLedgeCollision = false;
    [Header("Open Spelling (optional)")]
    public OpenSpellingLedge openSpellingLedge;

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
            return;
        }
        if (other.CompareTag("OpenSpellingLedge"))
        {
            openSpellingLedgeCollision= true;
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
        if (other.CompareTag("OpenSpellingLedge"))
        {
            openSpellingLedgeCollision = false;
        }
    }

    // referenced from Unity GrabInteraction game object
    public void OnGrabSelected()
    {
        Debug.Log($"Grabbed block of letter {targetID}.");
        _isHeld = true;
    }

    /// <summary>
    /// Called by grab system when the cube is released (When Unselect())
    /// </summary>
    public void OnGrabReleased()
    {
        Debug.Log($"Released grab block of letter {targetID}");

        // check to see where cube location is - if it triggered collision with the proper slot

        _isHeld = false;
        if (_isSnapping) return;

        // var cubeBounds = ComputeWorldBounds();
        if (ledgeCollision) // previously checked targetLedge.IsInsideBoundary(cubeBounds)
        {
            CES.InvokeOnPlayerGrabRelease(targetID, listIndex);
        }
        else if(openSpellingLedgeCollision)
        {
            Debug.Log("(Open) Placing block on open spelling ledge.");


            CES?.InvokeOnAdjustLedgeTransformPositions(gameObject.transform);
            CES?.InvokeOnOpenSpellingLetterSelected(targetID);
        }
        else //TODO: destroy game object in open spelling to avoid duplicates in the letter blocks
        {
            Debug.Log($"Returning grab block {targetID} to starting position");
            ReturnToStartPosition();
        }
    }

    /// <summary>
    /// Returns a grab block to its original position before it was grabbed
    /// </summary>
    public void ReturnToStartPosition()
    {
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(_startPos, _startRot, returnDuration));
    }

}
