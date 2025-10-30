using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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

/// <summary>
/// InteractionModality defines the user's input mode that they use to interact with objects and Interactions
/// </summary>
public enum InteractionModality
{
    Hand,
    Controller,
    Stylus,
    None
}

public class CubeInteraction : MonoBehaviour
{
    CentralEventSystem CES;
    
    public Interaction interactionType;
    //public InteractionModality interactionModality;
    public GameObject cube;
    public Transform ledgeSpawnRoot;
    public TextMeshPro letterText;

    public string targetID;
    public Ledge targetLedge;
    private bool ledgeCollision = false;
    private bool hasTarget;

    [Header("Grab Settings")]
    // 
    public GameObject grabInteraction;
    // Allow collider and snap check on grab release
    public bool grabEnabled;
    // Cube must be inside the ledge boundary/collider on release to snap
    public bool requireCubeInsideLedgeBoundaryToSnap;

    [Header("Poke Settings")]
    // Poke interaction that is a child of the cube interaction
    public GameObject pokeInteraction;
    // Allows snapping immediately when poked
    public bool pokeEnabled;
    // Identifies different poke sources based on InteractionModality
    public string[] pokeSourceTags;

    [Header("Snap and Return")]
    public float snapDuration = 0.12f;
    public bool snapRotation = true;
    public AnimationCurve snapEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool returnToStartIfInvalid = true;
    public float returnDuration = 0.15f;
    [SerializeField] private Collider mainCollider;

    // ---- runtime state ----
    private Rigidbody _rb;
    private Vector3 _startPos;
    private Quaternion _startRot;
    private Coroutine _moveRoutine;
    private bool _isSnapping;
    private bool _isHeld; // set by OnGrabSelected/OnGrabReleased
    
    public CubeInteraction(string id)
    {
        targetID = id;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        _startRot = transform.rotation;

        //if (targetLedge == null && !string.IsNullOrEmpty(targetID) && LedgeManager.Instance != null)
        //{
            //if (LedgeManager.Instance.TryGet(targetID, out var found))
               // targetLedge = found;
       // }

        //SetInteraction();
        //cubeSpawnPosition = this.transform;
    }

    private void Start()
    {
        CES = CentralEventSystem.Instance;

        if (CES != null)
        {
            CES.OnInteractionTypeChange += UpdateCubeInteraction;
        }
    }

    private void UpdateCubeInteraction(Interaction interaction)
    {
        Debug.Log("Updating Cube Interaction modalities...");
        switch (interaction)
        {
            case Interaction.Grab:
                SetGrabActive();
                break;
            case Interaction.Poke:
                SetPokeActive();
                break;
            case Interaction.None:
                grabInteraction.SetActive(true);
                pokeInteraction.SetActive(true);
                Debug.Log("Grab set active and Poke set active. No interaction type defined.");
                break;
        }
    }

    private void SetPokeActive()
    {
        grabInteraction.SetActive(false);
        grabEnabled = false;
        pokeEnabled = true;
        pokeInteraction.SetActive(true);
        Debug.Log("Grab set active, Poke set inactive.");
    }

    private void SetGrabActive()
    {
        grabInteraction.SetActive(true);
        grabEnabled = true;
        pokeEnabled = false;
        pokeInteraction.SetActive(false);
        Debug.Log("Grab set active, Poke set inactive.");
    }

    /// <summary>
    /// Handles collisions with the cube. Registers poke interactions and handles selection logic from there.
    /// </summary>
    /// <param name="other">The Collider that has collided with the cube collider </param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected a collision.");
        if (_isSnapping || targetLedge == null) return;


        // Is this a poke source?
        if (IsPokeSource(other) && !grabEnabled)
        {
            Debug.Log("Poked cube");
            // Poke logic: immediate snap to its matching ledge
            SnapToLedge(Interaction.Poke);
        }
        // check of other collider is a ledge and set a flag to that
        if (other.CompareTag("Ledge"))
        {
            Debug.Log("Cube collided with ledge.");
            ledgeCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // check of other collider is a ledge and set a flag to that
        if (other.CompareTag("Ledge"))
        {
            Debug.Log("Cube exited ledge collider.");
            ledgeCollision = false;
        }
    }

    private bool IsPokeSource(Collider other)
    {
        return ((1 << other.gameObject.layer)) != 0 || other.CompareTag("PokePointer");
    }

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
        Debug.Log($"Released cube of letter {targetID}");

        // check to see where cube location is - if it triggered collision with the proper slot

        _isHeld = false;
        if (!grabEnabled || _isSnapping || targetLedge == null) return;

        var cubeBounds = ComputeWorldBounds();
        bool inside = targetLedge.IsInsideBoundary(cubeBounds);
        if (!requireCubeInsideLedgeBoundaryToSnap || inside)
        {
            HandleGrabInteraction(targetLedge.transform);
        }
        else if (returnToStartIfInvalid)
        {
            Debug.Log($"Collision with incorrect ledge. Returning cube {targetID} to starting position"); 
            ReturnToStartPosition(); 
        }     
    }

    // invoked by correctplayerletterselection
    private void HandleGrabInteraction(Transform ledgeTransform)
    {
        bool correctSelection = CorrectLetterSelected();
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
            SnapToLedge(Interaction.Grab);
        }
    }

    // TODO - Invoke OnPlayerLetterSelected 
    private bool CorrectLetterSelected()
    {
        CES.InvokeOnPlayerLetterSelection(targetID);
        return true;
    }

    // TODO - delete this
    private void SnapToLedge(Interaction mode)
    {
        if (targetLedge?.snapPoint == null) return;

        float cubeHeight = mainCollider.bounds.size.y;

        Debug.Log($"cubeHeight for {mode.ToString()} is {cubeHeight}");

        // Adjust the snap position to sit just above the ledge
        Vector3 snapPosition = targetLedge.snapPoint.position + new Vector3(0f, cubeHeight / 2f, 0f);

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(snapPosition,
                                             snapRotation ? targetLedge.snapPoint.rotation : transform.rotation,
                                             snapDuration));
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

        if (targetLedge?.snapPoint == null) return;

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(targetLedge.snapPoint.position,
                                             snapRotation ? targetLedge.snapPoint.rotation : transform.rotation,
                                             snapDuration));
    }

    private void ReturnToStartPosition()
    {
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(_startPos, _startRot, returnDuration));
    }

    private IEnumerator MoveTo(Vector3 pos, Quaternion rot, float duration)
    {
        _isSnapping = true;              

        bool prevKinematic = _rb.isKinematic;
        _rb.isKinematic = true;

        Vector3 fromPos = transform.position;
        Quaternion fromRot = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            float e = snapEase.Evaluate(Mathf.Clamp01(t));
            transform.position = Vector3.LerpUnclamped(fromPos, pos, e);
            transform.rotation = Quaternion.SlerpUnclamped(fromRot, rot, e);
            yield return null;
        }

        transform.position = pos;
        transform.rotation = rot;

        _rb.isKinematic = prevKinematic;
        _isSnapping = false;
    }


    // Single-collider-friendly bounds (matches your current prefab)
    private Bounds ComputeWorldBounds()
    {
        var col = GetComponent<Collider>();
        if (col != null) return col.bounds;

        var r = GetComponent<Renderer>();
        if (r != null) return r.bounds;

        return new Bounds(transform.position, Vector3.one * 0.1f);
    }

    // Optional public reset
    public void ResetHome()
    {
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        transform.SetPositionAndRotation(_startPos, _startRot);
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _isHeld = false;
        _isSnapping = false;
    }


    public void InteractionSelected(string interaction)
    {
        Debug.Log($"User selected {interaction} interaction. Setting CubeInteraction.interactionType to {interaction}...");
        interactionType = (Interaction)Enum.Parse(typeof(Interaction), interaction);

        //UpdateCubeInteraction();
    }

}
