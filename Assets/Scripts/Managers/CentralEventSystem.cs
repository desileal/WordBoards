using System;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CentralEventSystem : MonoBehaviour
{
 
    public static CentralEventSystem Instance { get; private set; }

    #region Event Declaration

    public event Action<char> OnPlayerCubePoke;
    public event Action<Interaction> OnInteractionTypeChange;
    public event Action<char> OnPlayerCubeReleased;
    public event Action<char> OnPlayerLetterSelection;
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

    public void InvokeOnPlayerCubePoke(char c)
    {
        OnPlayerCubePoke?.Invoke(c);
    }

    public void InvokeOnInteractionTypeChange(Interaction interaction)
    {
        OnInteractionTypeChange?.Invoke(interaction);
    }

    public void InvokeOnPlayerCubeReleased(char c)
    {
        OnPlayerCubeReleased?.Invoke(c);
    }

    public void InvokeOnPlayerLetterSelection(char c)
    {
        OnPlayerLetterSelection?.Invoke(c);
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
