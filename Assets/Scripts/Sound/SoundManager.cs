using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private bool enableSound = true;

    // load
    public string path = "Assets/Sounds";

    // audio source
    public int sourceIndex = 0;
    public Transform sourceParent;
    public AudioSource[] fxSources;

    // audio source
    public int loopSourceIndex = 0;
    public Transform loopSourceParent;
    public AudioSource[] loopFxSources;

    // ambiance source
    public AudioSource ambiantSource;
    public AudioSource rainSource;

    public List<Sound> sounds = new List<Sound>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        EnableSound = true;

        fxSources = sourceParent.GetComponentsInChildren<AudioSource>();
        loopFxSources = loopSourceParent.GetComponentsInChildren<AudioSource>();

        UpdateAmbianceSound();
    }

    #region play functions
    public void PlayLoop(string soundName)
    {
        Sound sound = sounds.Find(x => x.name == soundName);

        if (sound == null)
        {
            Debug.LogError("no sound named : " + soundName);
            return;
        }

        loopFxSources[loopSourceIndex].clip = sound.clip;
        loopFxSources[loopSourceIndex].Play();

        ++loopSourceIndex;

        if (loopSourceIndex == loopFxSources.Length)
        {
            loopSourceIndex = 0;
        }
    }

    public void StopLoop(string soundName)
    {
        foreach (var item in loopFxSources)
        {
            if (item.isPlaying && item.clip.name == soundName)
            {
                item.Stop();
                return;
            }
        }

        Debug.LogError("coudln't find loop sound named : " + soundName);
    }
    public void PlayRandomSound(string soundName)
    {
        List<Sound> tmp_Sounds = sounds.FindAll(x => x.name.StartsWith(soundName));

        if (tmp_Sounds.Count == 0)
        {
            Debug.LogError("no random sound named : " + soundName);
            return;
        }

        Sound sound = tmp_Sounds[Random.Range(0, tmp_Sounds.Count)];
        PlaySound(sound);

    }
    public void PlaySound(string soundName)
    {
        Sound sound = sounds.Find(x => x.name == soundName);

        if (sound == null)
        {
            Debug.LogError("no sound named : " + soundName);
            return;
        }

        PlaySound(sound);
    }
    private void PlaySound (Sound sound)
    {
        fxSources[sourceIndex].clip = sound.clip;
        fxSources[sourceIndex].Play();

        ++sourceIndex;

        if (sourceIndex == fxSources.Length)
        {
            sourceIndex = 0;
        }
    }
    #endregion

    #region ambiance
    public void PlayAmbiance(string ambianceName)
    {
        Sound sound = sounds.Find(x => x.name == ambianceName);

        if (sound == null)
        {
            Debug.LogError("no sound named : " + ambianceName);
            return;
        }

        if (ambiantSource.clip != null && ambiantSource.clip.name == ambianceName)
        {
            return;
        }

        ambiantSource.clip = sound.clip;
        ambiantSource.Play();
    }
    public void PlayRain()
    {
        rainSource.Play();
    }
    public void StopRain()
    {

    }
    #endregion

    #region load
    public void LoadSounds()
    {
        sounds.Clear();

        string path = Application.dataPath + "/Sounds";

        string[] fileTypes = new string[3] { "mp3", "ogg", "wav" };

        foreach (var fileType in fileTypes)
        {
            string[] audioClip_Paths = Directory.GetFiles(path, "*." + fileType, SearchOption.AllDirectories);

            foreach (string audioClip_Path in audioClip_Paths)
            {
                string assetPath = "Assets" + audioClip_Path.Replace(Application.dataPath, "").Replace('\\', '/');
                AudioClip audioClip = (AudioClip)AssetDatabase.LoadAssetAtPath(assetPath, typeof(AudioClip));

                Sound newSound = new Sound();
                newSound.name = audioClip.name;
                newSound.clip = audioClip;
                sounds.Add(newSound);
            }
        }

        Debug.Log("Loaded " + sounds.Count + " sounds");
    }
    #endregion

    public void SwitchEnableSound()
    {
        EnableSound = !EnableSound;
    }

    public bool EnableSound
    {
        get
        {
            return enableSound;
        }
        set
        {
            enableSound = value;
        }
    }

    #region ambiance
    public void UpdateAmbianceSound()
    {
        if(StoryLauncher.Instance== null)
        {
            return;
        }
        if (StoryLauncher.Instance.PlayingStory)
        {
            UpdateLandAmbianceSound();
        }
        else
        {
            UpdateSeaAmbianceSound();
        }

        
    }

    void UpdateSeaAmbianceSound()
    {
        if (TimeManager.Instance == null)
        {
            return;
        }

        rainSource.Stop();

        if ( TimeManager.Instance.raining)
        {
            PlayAmbiance("ambiance_sea_rain");
        }
        else if ( TimeManager.Instance.dayState == TimeManager.DayState.Day)
        {
            PlayAmbiance("ambiance_beach_day");
        }
        else
        {
            PlayAmbiance("ambiance_beach_night");
        }
    }

    void UpdateLandAmbianceSound()
    {
        if ( InGameBackGround.Instance == null) {
            return;
        }

        switch (InGameBackGround.Instance.currentType)
        {
            case InGameBackGround.Type.Island:
                if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_beach_night");
                }
                else
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_beach_day");
                }
                break;
            case InGameBackGround.Type.House:
                SoundManager.Instance.PlayAmbiance("ambiance_house");
                break;
            case InGameBackGround.Type.Tavern:
                if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_tavern_night");

                }
                else
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_tavern_day");

                }
                break;
            case InGameBackGround.Type.Cave:
                SoundManager.Instance.PlayAmbiance("ambiance_cave");
                break;
            case InGameBackGround.Type.Forest:
                if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_forest_night");
                }
                else
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_forest_day");
                }
                break;
            case InGameBackGround.Type.Village:
                if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_village_night");

                }
                else
                {
                    SoundManager.Instance.PlayAmbiance("ambiance_village_day");

                }
                break;
            case InGameBackGround.Type.Boat:
                // same sea ambiance
                break;
        }

        if (TimeManager.Instance.raining)
        {
            rainSource.Play();
        }
        else
        {
            rainSource.Stop();
        }
    }
    #endregion
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}