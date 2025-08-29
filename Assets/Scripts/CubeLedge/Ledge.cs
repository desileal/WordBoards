using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Ledge : MonoBehaviour
{

    private Transform ledgePosition;
    private bool isCorrect;

    public string dictationLetter { get; private set; }
    public LedgeSlot slot { get; private set; }
    public int ledgeIndex { get; private set; }
}
