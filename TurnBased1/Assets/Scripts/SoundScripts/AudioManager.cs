using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
/*    [Range(0, 1)] public float musicVolumeModifier;
    [Range(0, 1)] public float soundVolumeModifier;*/

    public bool muteMusic;
    public bool muteSound;

    [SerializeField]
    public List<Sound> music;

    [SerializeField]
    public List<Sound> sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.clip = s.clip;
            s.source.loop = true;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.clip = s.clip;
        }
    }

    public void PlayMusic(string musicName)
    {
        Sound s = new Sound();

        for (int i = 0; i < music.Count; i++)
        {
            if (music[i].soundName == musicName)
            {
                s = music[i];
                break;
            }
        }

        if (!muteMusic)
            s.source.Play();
    }

    public void StopMusic(string musicName)
    {
        Sound s = new Sound();

        for (int i = 0; i < music.Count; i++)
        {
            if (music[i].soundName == musicName)
            {
                s = music[i];
                break;
            }
        }

        s.source.Stop();
    }


    public void PlaySound(string soundName)
    {
        Sound s = new Sound();
        
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].soundName == soundName)
            {
                s = sounds[i];
                break;
            }
        }
        
        if (!muteSound)
            s.source.Play();
    }
}
