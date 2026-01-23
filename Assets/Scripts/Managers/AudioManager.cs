using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance => instance;

    // Letterboard sounds.
    public List<AudioClip> letters = new List<AudioClip>();
    public List<AudioClip> punctuations = new List<AudioClip>();
    private Dictionary<string, AudioClip> audioMap;

    // State-specific dialogue.
    public List<AudioClip> dialogue = new List<AudioClip>();
    private Dictionary<string, AudioClip> dialogueMap;

    // Encouragements, Prompts, and Positive Reinforcements.
    public List<AudioClip> prompts = new List<AudioClip>();
    private Dictionary<string, AudioClip> promptMap;

    public void Awake()
    {
        // Create Singleton instance.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        // Populate audio map.
        audioMap = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in letters)
        {
            audioMap[clip.name] = clip;
        }

        foreach (AudioClip clip in punctuations)
        {
            audioMap[clip.name] = clip;
        }

        // Populate dialogue map.
        dialogueMap = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in dialogue)
        {
            dialogueMap[clip.name] = clip;
        }

        // Populate prompt map.
        promptMap = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in prompts)
        {
            promptMap[clip.name] = clip;
        }
    }

    public AudioClip GetClip(string name)
    {
        if (audioMap.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }
        else
        {
            Debug.LogError("AudioManager.GetClip() failed to find the named clip: " + name);

            return null;
        }
    }

    public AudioClip GetDialogue(string name)
    {
        if (dialogueMap.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }
        else
        {
            Debug.LogError("AudioManager.GetDialogue() failed to find the named clip: " + name);

            return null;
        }
    }

    public AudioClip GetPrompt(string name)
    {
        if (promptMap.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }
        else
        {
            Debug.LogError("AudioManager.GetPrompt() failed to find the named clip: " + name);

            return null;
        }
    }
}
