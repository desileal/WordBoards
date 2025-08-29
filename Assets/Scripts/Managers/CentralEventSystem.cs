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
    public event Action OnLedgeCollision;

    #endregion

    #region Event wrappers for external use

    public void InvokeOnPlayerCubePoke(char c)
    {
        OnPlayerCubePoke?.Invoke(c);
    }

    #endregion
}
