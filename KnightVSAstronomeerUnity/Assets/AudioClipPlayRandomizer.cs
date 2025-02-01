using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioClipPlayRandomizer : MonoBehaviour
{
    public List<AudioClip> clips;

    [InspectorRange(0, 1)]
    public float volume = 1f;

    [InspectorRange(-3, 3)]
    public float pitchMinRandomValue=1, pitchMaxRandomValue=1;

    public void PlayRandom(AudioSource source = null)
    {
        if(source == null)
            source = GetComponent<AudioSource>();
        source.clip = clips[Random.Range(0, clips.Count)];
        source.volume = volume;
        source.pitch = Random.Range(pitchMinRandomValue, pitchMaxRandomValue);
        source.Play();
    }
}
