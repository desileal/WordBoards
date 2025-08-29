using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

/// <summary>
/// Interaction defines the mode of selection for the user and determines how they will interact with the system.
/// Only one interaction type can be set at a time.
/// </summary>
public enum Interaction
{
    Grab,
    Poke,
    None
}


public enum InteractionModality
{
    Hand,
    Controller,
    Stylus,
    None
}

public class CubeInteraction : MonoBehaviour
{
    
    public Interaction interactionType;
    public InteractionModality interactionModality;
    public GameObject cube;
    public Transform ledgeSpawnRoot;
    public TextMeshPro letterText;

    // location for the corresponding slot
    [SerializeField]
    GameObject grabInteraction;
    [SerializeField]
    GameObject pokeInteraction;


    public Transform cubeSpawnPosition;
    
    [SerializeField]
    MaterialCycler materialCycler;

    private void Start()
    {
        SetInteractionType();
        cubeSpawnPosition = this.transform;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetInteractionType()
    {
        interactionModality = (InteractionModality)Random.Range(0, System.Enum.GetValues(typeof(InteractionModality)).Length);
        Debug.Log($"Setting Interaction modality to {interactionModality.ToString()}");
        interactionType = (Interaction)Random.Range(0, System.Enum.GetValues(typeof(Interaction)).Length);
        Debug.Log($"Setting interaction type to {interactionType.ToString()}");
        UpdateCubeInteraction();
    }

    private void UpdateCubeInteraction()
    {
        switch (interactionType)
        {
            case Interaction.Grab:
                grabInteraction.SetActive(true); 
                pokeInteraction.SetActive(false);
                break;
            case Interaction.Poke:
                grabInteraction.SetActive(false);
                pokeInteraction.SetActive(true);
                break;
            case Interaction.None:
                grabInteraction.SetActive(true);
                pokeInteraction.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other">The Collider that has collided with the cube collider </param>
    private void OnTriggerEnter(Collider other)
    {
        /*
        switch (interactionType)
        {
            case Interaction.Grab:
                if(other.CompareTag("Ledge"))
                {
                    HandleGrabInteraction(other.transform);
                }
                break;
            case Interaction.Poke:
                //if(other.CompareTag("InputController"))
                Rigidbody sourceRigidbody = other.attachedRigidbody;
                if (sourceRigidbody == null) return;
                GameObject sourceObject = sourceRigidbody.gameObject;
                if (other.CompareTag("InputController") || (sourceObject.name.Contains("Hand") && sourceObject.name.Contains("Rigidbody")))
                {
                    HandlePokeInteraction();
                }
                break;
            default: break;
        }
        */
        // if poke, check that it is the proper letter in word sequence

        // if not poke, we check the other collider to see if it is in the correct slot for spelling order
    }

    /// <summary>
    /// Called by grab system when the cube is released (When Unselect())
    /// </summary>
    public void OnGrabReleased()
    {
        Debug.Log("Released cube");

        // check to see where cube location is - if it triggered collision with the proper slot


    }


    private void HandleGrabInteraction(Transform ledgeTransform)
    {
        bool correctSelection = CorrectLetterSelected();
        // if letter is correct to corresponding ledge object, snap to ledge and set inactive
        if (!correctSelection)
        {
            // else put cube back to root spawn location
            ResetToStartingPosition();
        }
        else
        {
            SnapToLedge(ledgeTransform);
        }
    }

    private void HandlePokeInteraction()
    {
        // if letter is correct to corresponding ledge object, snap to ledge and set inactive
        // else put cube back to root spawn location
        SnapToLedge(ledgeSpawnRoot);
    }

    private bool CorrectLetterSelected()
    {

        return false;
    }


    public void OnPoke()
    {
        Debug.Log("Poked cube");

        materialCycler.CycleMaterial();

        //AddCubeToLedge();
        // update sentence
    }
    


    /// <summary>
    /// When a cube is dragged and released into the proper slot that the letter is intended to be to spell the set word.
    /// 
    /// </summary>
    private void SnapToLedge(Transform ledgeTransform)
    {
        // TODO
        // get the cube that user is interacting with and place it in proper location
        cube.SetActive(false);
        cube.transform.position = ledgeTransform.position;
        // reset rotation so that it is the original rotation
        // get ledge position and add (0.55 * cube height) so that it sits above ledge
    }

    private void ResetToStartingPosition()
    {
        this.transform.SetPositionAndRotation(cubeSpawnPosition.position, cubeSpawnPosition.rotation);
    }

}
