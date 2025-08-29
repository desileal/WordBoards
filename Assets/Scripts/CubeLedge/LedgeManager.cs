using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all the active ledges in the scene, updating them each time a new word is generated.
/// Each ledge corresponds to a specific letter in spelling order.
/// </summary>
public class LedgeManager : MonoBehaviour
{
    // Singleton for easy access
    public static LedgeManager Instance;

    private Dictionary<string, Transform> ledges = new Dictionary<string, Transform>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"> letter that corresponds with the correct position in the dictation word </param>
    /// <param name="ledgeTransform"> transform for the ledge to be positioned </param>
    public void RegisterLedge(string letter, Transform ledgeTransform)
    {

    }

    public void ClearLedges()
    {
        ledges.Clear();
    }
}
