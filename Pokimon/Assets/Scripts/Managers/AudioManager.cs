using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource;

    public Vector2 pitchRange = Vector2.zero;
    public static AudioManager SharedInstance;

    private void Awake()
    {
        if (SharedInstance != null)
        {
            Destroy(this);
        }else
        {
            SharedInstance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();
    }
    
    public void PlayMusic(AudioClip clip)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIdx = UnityEngine.Random.Range(0, clips.Length);
        float randomPitch = UnityEngine.Random.Range(pitchRange[0], pitchRange[1]);

        sfxSource.pitch = randomPitch;
        PlaySound(clips[randomIdx]);
    }
}
