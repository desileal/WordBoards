using UnityEngine;

public class PokeBlock : CubeInteraction
{
    public PokeBlock(string id) : base(id)
    {
        targetID = id;
    }

    /// <summary>
    /// Handles collisions with the cube. Registers poke interactions and handles selection logic from there.
    /// </summary>
    /// <param name="other">The Collider that has collided with the cube collider </param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Poke block detected a collision.");
        if (_isSnapping) return;


        // Is this a poke source?
        if (IsPokeSource(other))
        {
            Debug.Log($"Poked cube of letter, {targetID}");
            CES.InvokeOnPlayerLetterSelection(targetID, listIndex);
        }

    }


    private bool IsPokeSource(Collider other)
    {
        return ((1 << other.gameObject.layer)) != 0 || other.CompareTag("PokePointer");
    }
}
