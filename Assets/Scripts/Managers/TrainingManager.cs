using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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

    [SerializeField]
    private string wordsUrl = "https://desileal.github.io/WordBoards/words.json";

    // TODO: delete these?
    private readonly List<PhaseStep> trainingSteps = new();
    private readonly List<TrainingPhase> trainingPhases = new();

    [SerializeField] public List<string> warmupWords = new();
    // warmup, three, four and five letter words that users progress through during training
    [SerializeField] public List<string> trainingWords = new List<string>(12);
    // five, six and seven letter words that users spell with all letters
    [SerializeField] public List<string> testWords = new();
    private int currWordIndex;

    public List<string> introWords = new List<string>();
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
    IEnumerator Start()
    {
        CES = CentralEventSystem.Instance;

        yield return LoadWordsFromWeb();

        InitializeTraining();
        currWordIndex = 0;

        if (CES != null)
        {
            CES.OnWarmupStart += StartWarmup;
            CES.OnStepComplete += SetNextTrainingStep;
            CES.OnNextStep += NextTrainingStep;
            CES.OnNextStep += GetNextStepWord;
            CES.OnNextTrainingPhase += NextTrainingPhase;
            CES.OnTrainingEnd += EndTraining;
            CES.OnTrainingStart += StartTraining;
        }
    }

    private IEnumerator LoadWordsFromWeb()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(wordsUrl))
        {
            yield return www.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("Error loading words.json: " + www.error);
                yield break;
            }

            try
            {
                string json = www.downloadHandler.text;
                WordPhaseWrapper wrapper = JsonUtility.FromJson<WordPhaseWrapper>(json);
                if (wrapper != null)
                {
                    introWords = ConvertToUpper(wrapper.warmup ?? new List<string>());
                    fourLetterWords = ConvertToUpper(wrapper.fourLetter ?? new List<string>());
                    fiveLetterWords = ConvertToUpper(wrapper.fiveLetter ?? new List<string>());
                    challengeWords = ConvertToUpper(wrapper.challenge ?? new List<string>());
                }
                else
                {
                    Debug.LogError("words.json has wrong format");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse words.json: " + e);
            }
            // Merge warmup into warmupWords
            warmupWords.Clear();
            warmupWords.AddRange(introWords);
            // Merge all into trainingWords
            trainingWords.Clear();
            trainingWords.AddRange(fourLetterWords);
            trainingWords.AddRange(fiveLetterWords);
            trainingWords.AddRange(challengeWords);
        }
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
            CES.OnTrainingEnd -= EndTraining;
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

    private void StartWarmup()
    {
        if (warmupWords.Count == 0 || warmupWords == null)
        {
            Debug.LogError("No warmup words found - cannot start warmup");
            return;
        }
        currWordIndex = 0;
        currentTrainingPhase = TrainingPhase.Warmup;
        currentTrainingStep = PhaseStep.StepOne;
        CES.InvokeSetNextStepWord(warmupWords[currWordIndex]);
    }

    // initializes training when user is ready
    private void StartTraining()
    {
        if (trainingWords == null || trainingWords.Count == 0)
        {
            Debug.LogError("No training words loaded – cannot start training.");
            return;
        }
        currWordIndex = 0;
        currentTrainingPhase = TrainingPhase.ThreeLetters;
        currentTrainingStep = PhaseStep.StepOne;
        CES.InvokeSetNextStepWord(trainingWords[currWordIndex]);
    }

    // Sets the next training step depending on the current one
    // If training is in the final phase (FiveLetters) and final step (StepThree), training is complete
    private void SetNextTrainingStep ()
    {
        if(currentTrainingPhase == TrainingPhase.Warmup && currWordIndex >= warmupWords.Count)
        {
            CES.InvokeOnTrainingEnd();
            return;
        }
        if (currWordIndex == trainingWords.Count - 1)
        {
            Debug.Log("Training complete");
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
        /*
                if(currWordIndex >= trainingWords.Count)
                {
                    Debug.Log("No more training words.");
                    return;
                }
        */
        List<string> sourceList;

        // Decide which list to use
        if (currentTrainingPhase == TrainingPhase.Warmup)
        {
            sourceList = warmupWords;
        }
        else
        {
            sourceList = trainingWords;
        }

        // Check bounds
        if (currWordIndex >= sourceList.Count)
        {
            Debug.Log("No more words in this phase.");
            CES.InvokeOnTrainingEnd();
            return;
        }

        CES.InvokeSetNextStepWord(sourceList[currWordIndex]);

        CES.InvokeSetNextStepWord(trainingWords[currWordIndex]);
    }

    // TODO: play music or display a message congratulating user on finishing
    private void EndTraining ()
    {
        
    }
}
