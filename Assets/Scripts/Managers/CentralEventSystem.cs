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
    public event Action<string> OnPlayerCubePoke;
    public event Action<Interaction> OnInteractionTypeChange;
    public event Action<string> OnPlayerCubeReleased;
    public event Action<string> OnPlayerLetterSelection;
    public event Action OnNextStep;
    public event Action<string> OnSetStepWord;
    public event Action OnStepComplete;
    public event Action OnTrainingStart;
    public event Action OnNextTrainingPhase;
    public event Action OnTrainingEnd;
    public event Action OnTestingStart;
    public event Action OnTestingEnd;

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

    public void InvokeOnPlayerCubePoke(string s)
    {
        OnPlayerCubePoke?.Invoke(s);
    }

    public void InvokeOnInteractionTypeChange(Interaction interaction)
    {
        OnInteractionTypeChange?.Invoke(interaction);
    }

    public void InvokeOnPlayerCubeReleased(string s)
    {
        OnPlayerCubeReleased?.Invoke(s);
    }

    public void InvokeOnPlayerLetterSelection(string s)
    {
        OnPlayerLetterSelection?.Invoke(s);
    }

    public void InvokeOnNextStep()
    {
        OnNextStep?.Invoke();
    }

    public void InvokeSetNextStepWord(string s)
    {
        OnSetStepWord?.Invoke(s);
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

    #endregion
}
