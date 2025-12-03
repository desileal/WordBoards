using System;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CentralEventSystem : MonoBehaviour
{
 
    public static CentralEventSystem Instance { get; private set; }

    #region Event Declaration

    public event Action<Vector3> OnSetCubeSpawnAnchor;
    public event Action<Vector3> OnSetLedgeSpawnAnchor;
    public event Action<Quaternion> OnSetRotationAnchor;
    public event Action<string, int> OnPlayerCubePoke;
    public event Action<string> OnInteractionTypeChange;
    public event Action<int> OnLedgeCollision;
    public event Action<string, int> OnPlayerGrabRelease;
    public event Action OnNextStep;
    public event Action<string> OnSetStepWord;
    public event Action<string> OnNextStepTask;
    public event Action OnStepComplete;
    public event Action OnTrainingStart;
    public event Action OnNextTrainingPhase;
    public event Action OnTrainingEnd;
    public event Action OnTestingStart;
    public event Action OnTestingEnd;
    public event Action<Transform> OnAdjustLedgeTransformPositions;
    public event Action OnUpdateLedgeLetterTransforms;
    public event Action<string> OnOpenSpellingLetterSelected;

    #endregion

    #region Event wrappers for external use

    public void InvokeOnSetCubeSpawnAnchor(Vector3 anchor)
    {
        OnSetCubeSpawnAnchor?.Invoke(anchor);
    }

    public void InvokeOnSetLedgeSpawnAnchor(Vector3 anchor)
    {
        OnSetLedgeSpawnAnchor?.Invoke(anchor);
    }

    public void InvokeOnSetRotationAnchor(Quaternion anchor)
    {
        OnSetRotationAnchor?.Invoke(anchor);
    }

    public void InvokeOnPlayerCubePoke(string s, int i)
    {
        OnPlayerCubePoke?.Invoke(s, i);
    }

    public void InvokeOnInteractionTypeChange(string interaction)
    {
        OnInteractionTypeChange?.Invoke(interaction);
    }

    public void InvokeOnLedgeCollision(int ledgeIndex)
    {
        OnLedgeCollision?.Invoke(ledgeIndex);
    }

    public void InvokeOnPlayerGrabRelease(string s, int i)
    {
        OnPlayerGrabRelease?.Invoke(s, i);
    }

    public void InvokeOnNextStep()
    {
        OnNextStep?.Invoke();
    }

    public void InvokeSetNextStepWord(string s)
    {
        OnSetStepWord?.Invoke(s);
    }

    public void InvokeOnNextStepTask(string s)
    {
        OnNextStepTask?.Invoke(s);
    }

    public void InvokeOnStepComplete()
    {
        OnStepComplete?.Invoke();
    }

    public void InvokeOnTrainingStart()
    {
        OnTrainingStart?.Invoke();
    }

    public void InvokeOnNextTrainingPhase ()
    {
        OnNextTrainingPhase?.Invoke();
    }

    public void InvokeOnTrainingEnd()
    {
        OnTrainingEnd?.Invoke();
    }

    public void InvokeOnTestingStart()
    {
        OnTestingStart?.Invoke();
    }

    public void InvokeOnTestingEnd()
    {
        OnTestingEnd?.Invoke();
    }

    public void InvokeOnAdjustLedgeTransformPositions(Transform t)
    {
        OnAdjustLedgeTransformPositions?.Invoke(t);
    }

    public void InvokeOnUpdateLedgeLetterTransforms()
    {
        OnUpdateLedgeLetterTransforms?.Invoke();
    }

    public void InvokeOnOpenSpellingLetterSelected(string s) 
    {
        OnOpenSpellingLetterSelected?.Invoke(s);
    }

    #endregion

    #region Awake method
    /// <summary>
    /// Singleton protection code (template)
    /// </summary>
    private void Awake()
    {
        Debug.Log("Awake in CES called.");
        if (Instance != null)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    #endregion
}
