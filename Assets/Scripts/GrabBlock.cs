using UnityEngine;

public class GrabBlock : CubeInteraction
{

    private bool ledgeCollision = false;

    public GrabBlock(string id) : base(id)
    {
        targetID = id;
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
            Debug.Log("Grab block collided with ledge.");
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

    // referenced from Unity GrabInteraction game object
    public void OnGrabSelected()
    {
        Debug.Log($"Grabbed cube.");
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
        if (!grabEnabled || _isSnapping) return;

        // var cubeBounds = ComputeWorldBounds();
        if (ledgeCollision) // previously checked targetLedge.IsInsideBoundary(cubeBounds)
        {
            CES.InvokeOnPlayerLetterSelection(targetID, listIndex);
        }
        else
        {
            Debug.Log($"Returning grab block {targetID} to starting position");
            ReturnToStartPosition();
        }
    }


    public void ReturnToStartPosition()
    {
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(_startPos, _startRot, returnDuration));
    }

    // invoked by correctplayerletterselection
    private void HandleGrabInteraction()
    {
        //bool correctSelection = CorrectLetterSelected();
        // if letter is correct to corresponding ledge object, snap to ledge and set inactive
        if (!ledgeCollision)
        {
            // else put cube back to root spawn location
            Debug.Log("Resetting cube to starting position.");
            ReturnToStartPosition();
            //ResetToStartingPosition();
        }
        else
        {
            SnapToLedge(targetLedge.snapPoint);
        }
    }

}
