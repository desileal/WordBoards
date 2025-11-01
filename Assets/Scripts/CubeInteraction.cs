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
/// InteractionModality defines the user's input mode that they use to interact with objects and Interactions
/// </summary>
public enum InteractionModality
{
    Hand,
    Controller,
    Stylus,
    None
}

/// <summary>
/// 
/// </summary>
public class CubeInteraction : MonoBehaviour
{
    protected CentralEventSystem CES;
    
    public Interaction interactionType; // delete
    //public InteractionModality interactionModality;
    public GameObject cube;
    public TextMeshPro letterText; //delete 

    // letter of the cube
    public string targetID;
    public Ledge targetLedge; //  delete ?
    protected bool hasTarget;

    protected int listIndex;

    [Header("Grab Settings")]
    // 
    public GameObject grabInteraction;
    // Allow collider and snap check on grab release
    public bool grabEnabled;

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
    protected Rigidbody _rb;
    protected Vector3 _startPos;
    protected Quaternion _startRot;
    protected Coroutine _moveRoutine;
    protected bool _isSnapping;
    protected bool _isHeld; // set by OnGrabSelected/OnGrabReleased
    
    public CubeInteraction(string id)
    {
        targetID = id;
        
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    private void Start()
    {
        CES = CentralEventSystem.Instance;

        if (CES != null)
        {
            CES.OnInteractionTypeChange += UpdateCubeInteraction;
        }
    }

    // TODO - make this modular to retrive from the session manager
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
        interactionType = Interaction.Poke;
        Debug.Log("Poke set active, Grab set inactive.");
    }

    private void SetGrabActive()
    {
        grabInteraction.SetActive(true);
        grabEnabled = true;
        pokeEnabled = false;
        pokeInteraction.SetActive(false);
        interactionType = Interaction.Grab;
        Debug.Log("Grab set active, Poke set inactive.");
    }

    // 
    public void SnapToLedge(Transform ledgeTransform)
    {

        if (ledgeTransform == null)
        {
            Debug.LogError("SnapToLedge received a null Ledge Transform");
            return;
        }

        float cubeHeight = mainCollider.bounds.size.y;

        Debug.Log($"cubeHeight is {cubeHeight}");

        // Adjust the snap position to sit just above the ledge
        Vector3 snapPosition = ledgeTransform.position + new Vector3(0f, cubeHeight / 2f, 0f);

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(snapPosition,
                                             snapRotation ? ledgeTransform.rotation : transform.rotation,
                                             snapDuration));
    }


    public IEnumerator MoveTo(Vector3 pos, Quaternion rot, float duration)
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
    public Bounds ComputeWorldBounds()
    {
        var col = GetComponent<Collider>();
        if (col != null) return col.bounds;

        var r = GetComponent<Renderer>();
        if (r != null) return r.bounds;

        return new Bounds(transform.position, Vector3.one * 0.1f);
    }

    // Optional public reset
    // TODO - delete
    public void ResetHome()
    {
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        transform.SetPositionAndRotation(_startPos, _startRot);
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _isHeld = false;
        _isSnapping = false;
    }

}
