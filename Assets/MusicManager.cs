using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource audioSource;

    public AudioClip[] combatMusics;
    public AudioClip[] baladMusic;

    public AudioClip tavernMusic;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayTavernMusic();
    }

    public void PlayCombatMusic()
    {
        if (!SoundManager.Instance.SoundEnabled)
        {
            return;
        }

        audioSource.loop = true;
        audioSource.clip = combatMusics[Random.Range(0, combatMusics.Length)];
        audioSource.Play();
    }

    public void StopCombatMusic()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }

    public void PlayBaladMusic()
    {
        if (!SoundManager.Instance.SoundEnabled)
        {
            return;
        }

        audioSource.clip = baladMusic[Random.Range(0, baladMusic.Length)];
        audioSource.Play();
    }

    public void PlayTavernMusic()
    {
        if (!SoundManager.Instance.SoundEnabled)
        {
            return;
        }

        audioSource.clip = tavernMusic;
        audioSource.Play();
    }
}
