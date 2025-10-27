using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    // contains full word and tracks within that word
    // next step is next word, next task is next letter

    [SerializeField] public GameObject cubeInteractionPrefab;

    // Word that the user is spelling
    private string _taskWord { get; set; }
    // array that is stepped through with each correct letter selected
    private string[] _wordLetters { get; set; }
    // tracks the index of the next letter to be spelled in the _wordLetters array
    private int _currentLetterIndex { get; set; }
    // Current letters that have been spelled
    private string[] _lettersSpelled;
    // Next letter that will invoke correct collision
    private string _nextLetter;

    private List<CubeInteraction> interactionBlocks = new();

    private Ledge blockLedge;

    CentralEventSystem CES;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CES = CentralEventSystem.Instance;

        if (CES != null)
        {
            CES.OnPlayerLetterSelection += UpdateLettersSpelled;
            CES.OnNextStep += ClearInteractionBlocks;
            CES.OnSetStepWord += SetStepTask;
        }
    }

    private void SetStepTask (string stepWord)
    {
        _taskWord = stepWord;
        _wordLetters = new[] { stepWord };
        _currentLetterIndex = 0;
        _lettersSpelled = new string[_wordLetters.Length];
        _nextLetter = _wordLetters[_currentLetterIndex];
        SpawnInteractionBlocks();
    }

    // initiate interaction blocks
    // randomize order
    // add extra letters
    // display word to be spelled

    private void ClearInteractionBlocks ()
    {
        foreach (var block in interactionBlocks)
        {
            Destroy(block);
        }
    }

    private void SpawnInteractionBlocks ()
    {
        foreach (string s in _wordLetters)
        {
            // create a block for letter
            // create a ledge for the block with corresponding target letter
        }
        RandomizeInteractionBlocks();
    }

    private void RandomizeInteractionBlocks ()
    {
        // randomize order of blocks that is displayed in UI
        // add more letters/random blocks through phase progression
    }

    // verify if cube poked or grab collision matches the next letter in the task


    //  Updates the Task with the letters that have been spelled
    private void UpdateLettersSpelled(char c)
    {
        _lettersSpelled.Append(c.ToString());
        _currentLetterIndex++;
        if (_currentLetterIndex <= _lettersSpelled.Length)
        {
            _nextLetter = _wordLetters[_currentLetterIndex];
        }
        else
        {
            CES.InvokeOnNextStep();
        }
    }



}
