using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

// Phases that user progresses through during training, increasing in complexity
public enum TrainingPhase
{
    NotStarted,
    Warmup,
    ThreeLetters,
    FourLetters,
    FiveLetters,
    Done
}

// There are three steps (word tasks) in each training phase
public enum PhaseStep
{
    StepOne,
    StepTwo,
    StepThree,
    None
}

/// <summary>
/// Training Manager is responsible for handling the progress through steps (words) in each training phase
/// Training phases include Warm up, three letter words, four letter words and five letter words
/// </summary>
public class TrainingManager : MonoBehaviour
{
    // current training step (1-3) that user is in the specific training phase
    private PhaseStep currentTrainingStep;
    // current training phase that user is in, starting from 3 letter words to 5 letter words
    private TrainingPhase currentTrainingPhase;

    // TODO: delete these?
    private readonly List<PhaseStep> trainingSteps = new();
    private readonly List<TrainingPhase> trainingPhases = new();

    // warmup, three, four and five letter words that users progress through during training
    [SerializeField] public List<string> trainingWords = new List<string>();
    // five, six and seven letter words that users spell with all letters
    [SerializeField] public List<string> testWords = new();
    private int currWordIndex;

    public List<string> warmupWords = new List<string>();
    public List<string> fourLetterWords = new List<string>();
    public List<string> fiveLetterWords = new List<string>();
    public List<string> challengeWords = new List<string>();

    CentralEventSystem CES;

    [Serializable]
    private class WordPhaseWrapper
    {
        public List<string> warmup;
        public List<string> fourLetter;
        public List<string> fiveLetter;
        public List<string> challenge;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        CES = CentralEventSystem.Instance;

        //yield return LoadWordsFromWeb();

        InitializeTraining();
        currWordIndex = 0;

        if (CES != null)
        {
            CES.OnSetSessionConfig += InitializeFromConfig;
            CES.OnStepComplete += SetNextTrainingStep;
            CES.OnNextStep += NextTrainingStep;
            CES.OnNextStep += GetNextStepWord;
            CES.OnNextTrainingPhase += NextTrainingPhase;
            CES.OnTrainingStart += StartTraining;
        }
    }

    public void InitializeFromConfig(SessionConfig config)
    {
        if (config == null)
        {
            Debug.LogError("InitializeFromConfig called with null SessionConfig.");
            return;
        }

        warmupWords = ConvertToUpper(config.warmup ?? new List<string>());
        warmupWords.Insert(0, config.startingInteraction.ToUpper());
        fourLetterWords = ConvertToUpper(config.fourLetter ?? new List<string>());
        fiveLetterWords = ConvertToUpper(config.fiveLetter ?? new List<string>());
        challengeWords = ConvertToUpper(config.challenge ?? new List<string>());
        
        trainingWords.Clear();
        trainingWords.AddRange(warmupWords);
        trainingWords.AddRange(fourLetterWords);
        trainingWords.AddRange(fiveLetterWords);
        trainingWords.AddRange(challengeWords);

        Debug.Log($"*** TrainingManager initialized with {trainingWords.Count} words ***");
    }

    private List<string> ConvertToUpper(List<string> words)
    {
        return words.ConvertAll(x => x.ToUpper());
    }

    // unsubscribe from all events
    private void OnDestroy()
    {
        if (CES)
        {
            CES.OnStepComplete -= SetNextTrainingStep;
            CES.OnNextStep -= NextTrainingStep;
            CES.OnNextStep -= GetNextStepWord;
            CES.OnNextTrainingPhase -= NextTrainingPhase;
            CES.OnTrainingStart -= StartTraining;
        }   
    }

    // TODO: delete? 
    private void InitializeTraining()
    {
        trainingPhases.Clear();
        trainingSteps.Clear();

        trainingPhases.Add(TrainingPhase.NotStarted);
        trainingPhases.Add(TrainingPhase.Warmup);
        trainingPhases.Add(TrainingPhase.ThreeLetters);
        trainingPhases.Add (TrainingPhase.FourLetters);
        trainingPhases.Add(TrainingPhase.FiveLetters);
        trainingPhases.Add(TrainingPhase.Done);

        trainingSteps.Add(PhaseStep.StepOne);
        trainingSteps.Add(PhaseStep.StepTwo);
        trainingSteps.Add(PhaseStep.StepThree);
    }

    // initializes training when user is ready
    private void StartTraining(string s)
    {
        if (trainingWords == null || trainingWords.Count == 0)
        {
            Debug.LogError("No training words loaded – cannot start training.");
            return;
        }

        trainingWords[0] = s.ToUpper();
        currentTrainingPhase = TrainingPhase.Warmup;
        currentTrainingStep = PhaseStep.StepOne;
        CES.InvokeSetNextStepWord(trainingWords[currWordIndex]);
    }

    // Sets the next training step depending on the current one
    // If training is in the final phase (FiveLetters) and final step (StepThree), training is complete
    private void SetNextTrainingStep ()
    {
        if (currWordIndex == trainingWords.Count - 1)
        {
            Debug.Log("Training complete");
            EndTraining();
            CES.InvokeOnTrainingEnd();
        }
        
        else
        {
            CES.InvokeOnNextStep();
        }
    }

    // Moves the training phase to the next training step
    // After StepThree is complete, the system moves to the next phase and begins at StepOne
    private void NextTrainingStep ()
    {
        switch (currentTrainingStep)
        {
            case PhaseStep.StepOne:
                currentTrainingStep = PhaseStep.StepTwo;
                break;
            case PhaseStep.StepTwo:
                currentTrainingStep = PhaseStep.StepThree;
                break;
            case PhaseStep.StepThree:
                currentTrainingStep = PhaseStep.StepOne;
                CES.InvokeOnNextTrainingPhase();
                break;
        }

    }

    // Determines the next phase of training after the current one is complete
    private void NextTrainingPhase()
    {
        switch (currentTrainingPhase) {
            case TrainingPhase.NotStarted:
                break;
            case TrainingPhase.Warmup:
                currentTrainingPhase = TrainingPhase.ThreeLetters;
                break;
            case TrainingPhase.ThreeLetters:
                currentTrainingPhase = TrainingPhase.FourLetters;
                break;
            case TrainingPhase.FourLetters:
                currentTrainingPhase = TrainingPhase.FiveLetters;
                break;
        }
    }


    private void GetNextStepWord ()
    {
        currWordIndex++;

        if(currWordIndex >= trainingWords.Count)
        {
            Debug.Log("No more training words.");
            return;
        }
        CES.InvokeSetNextStepWord(trainingWords[currWordIndex]);
    }

    // TODO: play music or display a message congratulating user on finishing
    private void EndTraining ()
    {
        currWordIndex = 0;
        currentTrainingPhase = TrainingPhase.Warmup;
        currentTrainingStep = PhaseStep.StepOne;

        // If you have any flags like `trainingStarted` or timers, reset them here too.
        Debug.Log("TrainingManager reset for new training run.");
    }
}
