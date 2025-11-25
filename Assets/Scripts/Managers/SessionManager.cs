using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Status of the system 
/// </summary>
public class SessionStatus
{
    private SessionModule sessionModule;
    private InteractionType interactionModality;
    private TrainingPhase trainingPhase;
    private PhaseStep trainingPhaseStep;
    // word being spelled
    private string stepWord;
    // current letter the user is trying to select
    private string currentTask;
    // letters the user has already selected
    private string completedTask;

    public SessionStatus()
    {

    }

    // session status when in training
    public SessionStatus (SessionModule sessionModule, InteractionType interactionModality, TrainingPhase trainingPhase, PhaseStep trainingPhaseStep, string stepWord, string currentTask, string completedTask)
    {
        this.sessionModule = sessionModule;
        this.interactionModality = interactionModality;
        this.trainingPhase = trainingPhase;
        this.trainingPhaseStep = trainingPhaseStep;
        this.stepWord = stepWord;
        this.currentTask = currentTask;
        this.completedTask = completedTask;
    }


}

public enum SessionModule
{
    InTraining,
    InTesting
}

/// <summary>
/// Interaction defines the mode of selection for the user and determines how they will interact with the system.
/// Only one interaction type can be set at a time.
/// </summary>
public enum InteractionType
{
    Grab,
    Poke,
    None
}

/// <summary>
/// 
/// </summary>
public class SessionManager : MonoBehaviour
{
    [SerializeField] private TrainingManager trainingManager;
    [SerializeField] private TestingManager testingManager;

    // object that holds current status to be serialized to dashboard
    private SessionStatus sessionStatus;

    public InteractionType currentInteractionType;

    public bool trainingIsReady = false;

    [SerializeField] public GameObject objectSpawnLocation;

    CentralEventSystem CES;


    // Delay start to check that the Training Phases are ready 
    private IEnumerator Start()
    {
        CES = CentralEventSystem.Instance;

        sessionStatus = new SessionStatus();
        currentInteractionType = InteractionType.None;

        if (CES == null)
        {
            yield return new WaitUntil(() => CES != null);
        }
        if (CES != null)
        {
            CES.OnNextStepTask += UpdateSessionStatus;
            //CES.OnInteractionTypeChange += SetSessionInteractionType;
        }
        if (trainingManager == null)
        {
            Debug.LogError("Missing reference to TrainingManager in SessionManager script.");
            yield break;
        }
        if (trainingManager.trainingWords != null)
        {
            trainingIsReady = true;
            CES.InvokeOnTrainingStart();
        }
        if (testingManager == null)
        {
            Debug.LogError("Missing reference to TestingManager in SessionManager script.");
            yield break;
        }
        if (testingManager)
        {
            CES.OnTrainingEnd += StartTesting;
        }
        else
        {
            
        }
    }

    // event fired every time user makes progress in task or phase
    // method subscribed to that event that passes session status game object and then updates the sessionStatus
    // bridge will also subscribe to same event, serialize and send it
    // TODO
    private void UpdateSessionStatus(string s)
    {

    }

    // called from Unity
    public void SetSessionInteractionType(string interaction)
    {
        Debug.Log($"Setting session interaction type to {interaction}");
        if (interaction.Equals("Poke")) currentInteractionType = InteractionType.Poke;
        else if (interaction.Equals("Grab")) currentInteractionType = InteractionType.Grab;
        else currentInteractionType = InteractionType.None;
    }

    // get x, y, z locations of top and bottom of the quad
    // top transform is the anchor for cube game objects
    // bottom transform is the anchor for ledge game objects
    public void SetObjectSpawnLocations ()
    {
        Bounds bounds = objectSpawnLocation.GetComponent<Renderer>().bounds;
        if (bounds == null)
        {
            Debug.LogError("Bounds are null");
        }
        float height = bounds.size.y;

        Vector3 centerHeight = bounds.center;

        Vector3 topAnchor = centerHeight + objectSpawnLocation.transform.up * (height / 2f);
        Vector3 bottomAnchor = centerHeight - objectSpawnLocation.transform.up * (height / 2f);
        Quaternion rotation = objectSpawnLocation.transform.rotation;
        // invoke set spawn locations in StepManager
        
        Debug.Log($"Setting object spawn locations for blocks at {topAnchor} and ledges at {bottomAnchor}");

        GameObject calibrateButton = GameObject.FindGameObjectWithTag("Calibration");
        Destroy( calibrateButton );

        Destroy(objectSpawnLocation);
        
        CES.InvokeOnSetCubeSpawnAnchor(topAnchor);
        CES.InvokeOnSetLedgeSpawnAnchor(bottomAnchor);
        CES.InvokeOnSetRotationAnchor(rotation);
        CES.InvokeOnTrainingStart(); // TODO: change this later, for testing purposes now
    }

    private void StartWarmup ()
    {

    }

    // Invoked after the spawning locations have been calibrated
    public void StartTraining()
    {
        CES?.InvokeOnTrainingStart();
    }

    // Starts testing phase through Testing Manager
    private void StartTesting ()
    {

    }

}
