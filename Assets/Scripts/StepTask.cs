using System.Linq;
using UnityEngine;

/// <summary>
/// Contains all the information for each task - CONTAINED IN STEP MANAGER
/// </summary>
public class StepTask : MonoBehaviour
{
    
    // Word that the user is spelling
    private string _taskWord { get; set; }
    // array that is stepped through with each correct letter selected
    private string[] _wordLetters { get; set; }
    // tracks the index of the next letter to be spelled in the _wordLetters array
    private int _currentLetterIndex {  get; set; }
    // Current letters that have been spelled
    public string[] lettersSpelled;
    // Next letter that will invoke correct collision
    public string nextLetter;

    CentralEventSystem CES;

    public StepTask(TrainingPhase trainingPhase, PhaseStep trainingStep, string taskWord, string[] wordLetters)
    {
        _taskWord = taskWord;
        _wordLetters = taskWord.ToCharArray().Select(c => c.ToString()).ToArray();
        _currentLetterIndex = 0;
        nextLetter = _wordLetters[_currentLetterIndex];
    }

    private void Start()
    {
        CES = CentralEventSystem.Instance;

        if (CES != null)
        {
            CES.OnPlayerLetterSelection += UpdateLettersSpelled;
        }
    }

    // Updates the Task with the letters that have been spelled
    // Invoked by OnPlayerLetterSelected
    public void UpdateLettersSpelled(char c)
    {
        lettersSpelled.Append(c.ToString());
        _currentLetterIndex++;
        if (_currentLetterIndex <= lettersSpelled.Length)
        {
            nextLetter = _wordLetters[_currentLetterIndex];
        }
        else
        {
            CES.InvokeOnNextStep();
        }
    }

    
}
