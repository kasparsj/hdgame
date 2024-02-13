using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;
 
public static class AudioFade
{
    public static IEnumerator In (AudioSource audioSource, float FadeTime, float targetVolume = 1) {
        float diff = targetVolume - audioSource.volume;
 
        while (audioSource.volume < targetVolume) {
            audioSource.volume += diff * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.volume = targetVolume;
    }
 
    public static IEnumerator Out (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.Stop ();
        audioSource.volume = startVolume;
    }
 
}
