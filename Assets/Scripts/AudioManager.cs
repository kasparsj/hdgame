using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] audioSources;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void ToggleAudioChannel(AudioSource audioSource)
    {
        // todo: check audioSource exists
        audioSource.mute = !audioSource.mute;
        if (!audioSource.mute && !audioSource.isPlaying) {
            StartCoroutine(StartAudioSources());
            StartCoroutine(SyncSources());
        }
    }

    private IEnumerator StartAudioSources()
    {
        foreach (var source in audioSources)
        {
            source.Play();
        }
        yield return null;
    }

    private IEnumerator SyncSources()
    {
        while (true)
        {
            if (audioSources.Length > 0)
            {
                // float time = audioSources[0].time;
                int timeSamples = audioSources[0].timeSamples;

                for (int i = 1; i < audioSources.Length; i++)
                {
                    // if (Mathf.Abs(audioSources[i].time - time) > 0.01f) {
                    if (Mathf.Abs(audioSources[i].timeSamples - timeSamples) > 100) {
                        audioSources[i].timeSamples = timeSamples;
                    }
                }
                yield return null;
            }
        }    
    }
}
