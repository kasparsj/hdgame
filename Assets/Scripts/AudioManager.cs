using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] audioSources;

    public bool PlayOnStart = false;
    public bool PauseOnStart = false;
    public bool[] Sync;

    private Coroutine playRoutine;
    private Coroutine syncRoutine;

    protected void Start()
    {
        if (PlayOnStart) {
            playRoutine = StartCoroutine(PlayAudioSources());
            syncRoutine = StartCoroutine(SyncSources());
        }
        if (PauseOnStart) {
            PauseAudioSources();
        }
    }

    protected void Update()
    {
    }

    public void ToggleAudioChannel(AudioSource audioSource)
    {
        // todo: check audioSource exists
        audioSource.mute = !audioSource.mute;
        if (!audioSource.mute) {
            if (playRoutine == null) {
                playRoutine = StartCoroutine(PlayAudioSources());
                syncRoutine = StartCoroutine(SyncSources());
            }
            else if (!audioSource.isPlaying) {
                audioSource.UnPause();
            }
        }
    }

    public void UnmuteAudioChannel(AudioSource audioSource)
    {
        // todo: check audioSource exists
        audioSource.mute = false;
        if (playRoutine == null) {
            playRoutine = StartCoroutine(PlayAudioSources());
            playRoutine = StartCoroutine(SyncSources());
        }
        else if (!audioSource.isPlaying) {
            audioSource.UnPause();
        }
    }

    private IEnumerator PlayAudioSources()
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
            AudioSource master = null;
            int timeSamples = 0;
            // float time = 0;

            for (var i=0; i<audioSources.Length; i++) {
                if (audioSources[i].isPlaying && Sync.Length > i && Sync[i]) {
                    if (master == null) {
                        master = audioSources[i];
                        // time = master.time;
                        timeSamples = master.timeSamples;
                    }
                    else {
                        if (Mathf.Abs(audioSources[i].timeSamples - timeSamples) > 100) {
                            audioSources[i].timeSamples = timeSamples;
                        }
                    }
                }
            }
            yield return null;
        }    
    }

    private void PauseAudioSources()
    {
        foreach (var source in audioSources)
        {
            source.Pause();
        }
    }
}
