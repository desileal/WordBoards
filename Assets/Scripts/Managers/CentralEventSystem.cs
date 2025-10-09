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
    public event Action OnInteractionTypeChange;

    #endregion

    #region Event wrappers for external use

    public void InvokeOnPlayerCubePoke(char c)
    {
        OnPlayerCubePoke?.Invoke(c);
    }

    public void InvokeOnInteractionTypeChange()
    {
        OnInteractionTypeChange?.Invoke();
    }

    #endregion
}
