using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;

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

public class SessionConfig
{
    public List<string> warmup;
    public List<string> fourLetter;
    public List<string> fiveLetter;
    public List<string> challenge;
    public string startingInteraction; // "poke" or "grab"
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
    [SerializeField]
    private string configUrl = "https://desileal.github.io/WordBoards/words.json";

    public SessionConfig currentSessionConfig;

    [SerializeField] private TrainingManager trainingManager;
    [SerializeField] private TestingManager testingManager;

    // object that holds current status to be serialized to dashboard
    private SessionStatus sessionStatus;

    // TODO: add this from git site so the order is randomized
    public InteractionType currentInteractionType;

    private int trainingRunsCompleted = 0;

    [SerializeField] public GameObject objectSpawnLocation;

    CentralEventSystem CES;


    // Delay start to check that the Training Phases are ready 
    private IEnumerator Start()
    {
        CES = CentralEventSystem.Instance;

        // wait until config file has been loaded from json
        yield return InitializeSession();

        sessionStatus = new SessionStatus();

        // Make sure config actually loaded
        if (currentSessionConfig == null)
        {
            Debug.LogError("Session config not loaded... cannot proceed with system function.");
            yield break;
        }

        if (CES == null)
        {
            yield return new WaitUntil(() => CES != null);
        }
        CES.OnNextStepTask += UpdateSessionStatus;
        if (trainingManager == null)
        {
            Debug.LogError("Missing reference to TrainingManager in SessionManager script.");
            yield break;
        }
        
        if (testingManager == null)
        {
            Debug.LogError("Missing reference to TestingManager in SessionManager script.");
            yield break;
        }
        trainingManager.InitializeFromConfig(currentSessionConfig);
        CES.OnTrainingEnd += HandleTrainingEnd;
    }

    private IEnumerator InitializeSession()
    {
        yield return LoadConfigFromWeb();

        if (currentSessionConfig == null)
        {
            Debug.LogError("No session config loaded, cannot continue.");
            yield break;
        }

        // Set starting interaction once
        SetSessionInteractionType(currentSessionConfig.startingInteraction);

        trainingRunsCompleted = 0;
        // Decide which mode to run 
        // todo later
        // StartTrainingMode();
        // or: StartOpenSpellingMode();
    }

    private IEnumerator LoadConfigFromWeb()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(configUrl))
        {
            yield return www.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("Error loading words.json: " + www.error);
                yield break;
            }

            try
            {
                string json = www.downloadHandler.text;
                currentSessionConfig = JsonUtility.FromJson<SessionConfig>(json);
                if (currentSessionConfig == null)
                {
                    Debug.LogError("Failed to parse session config JSON");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse words.json: " + e);
            }
        }
    }

    private InteractionType SwitchInteraction(InteractionType type)
    {
        if (type == InteractionType.Poke) return InteractionType.Grab;
        if (type == InteractionType.Grab) return InteractionType.Poke;
        return InteractionType.None;
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
        if (interaction.Equals("poke")) currentInteractionType = InteractionType.Poke;
        else if (interaction.Equals("grab")) currentInteractionType = InteractionType.Grab;
        else
        {
            Debug.LogWarning("No interaction type specified in website. Setting to default poke.");
            currentInteractionType = InteractionType.Poke;
        }
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
        StartTraining(); // TODO: change this later, for testing purposes now
    }

    // Invoked after the spawning locations have been calibrated
    public void StartTraining()
    {
        CES.InvokeOnTrainingStart(currentInteractionType.ToString());
    }

    private void HandleTrainingEnd()
    {
        trainingRunsCompleted++;
        Debug.Log($"Training run {trainingRunsCompleted} finished with interaction {currentInteractionType}");

        if (trainingRunsCompleted == 1)
        {
            // First run is done - switch to the other interaction and run training again
            currentInteractionType = SwitchInteraction(currentInteractionType);
            Debug.Log($"Starting second training run with interaction {currentInteractionType}");

            StartTraining();  // whatever you already use to kick off training
        }
        else
        {
            // Both interaction modes are done - go to testing or open spelling
            Debug.Log("Both training runs complete. Starting testing (or next phase).");
            StartTesting();   // or whatever your next phase is
        }
    }

    // Starts testing phase through Testing Manager
    private void StartTesting ()
    {

    }

}
