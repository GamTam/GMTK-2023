using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup group;
    [SerializeField] private List<Sound> sounds;
    
    public static SoundManager instance;
    public bool DoneLoading;

    void Start()
    {
        DoneLoading = false;
        DontDestroyOnLoad(gameObject);
        if (Globals.SoundManager != null)
        {
            DoneLoading = true;
            Destroy(gameObject);
            return;
        }
        
        Globals.SoundManager = this;
    }

    public IEnumerator LoadSounds() {

        Dictionary<string, ArrayList> musicDict = new Dictionary<string, ArrayList>();
        
        AsyncOperationHandle<TextAsset> tsvHandler = Addressables.LoadAssetAsync<TextAsset>("Assets/Audio/Sound Data.tsv");
        yield return tsvHandler;

        if (tsvHandler.Status == AsyncOperationStatus.Failed)
        {
            DoneLoading = true;
            yield break;
        }
        
        musicDict = Globals.LoadTSV(tsvHandler.Result);

        int i = 0;
        foreach(KeyValuePair<string, ArrayList> entry in musicDict) {
            if (i != 0) {
                Sound sound = new Sound();
                
                sound.name = entry.Key;

                String path = "Assets/Sound/" + sound.name + ".wav";
                
                AsyncOperationHandle<AudioClip> clipHandler = Addressables.LoadAssetAsync<AudioClip>(path);
                yield return clipHandler;

                if (clipHandler.Status == AsyncOperationStatus.Succeeded)
                {
                    sound.source = gameObject.AddComponent<AudioSource>();
                    sound.source.clip = clipHandler.Result;
                    sound.source.pitch = sound.pitch;
                    sound.source.loop = Convert.ToBoolean(entry.Value[0]);
                    sound.source.outputAudioMixerGroup = group;
                }

                sounds.Add(sound);
            }

            i++;
        }

        DoneLoading = true;
    }

    public void Stop(Sound s) {
        s.source.Stop();
    }

    public Sound Play (string name, float pitch = 1f)
    {
        Sound s = sounds.Find(x => x.name == name);
        if (s == null)
            return null;

        s.source.pitch = pitch;
        s.source.volume = 1f;
        s.source.PlayOneShot(s.source.clip);

        return s;
    }

    public void FadeOut(Sound sound, float length=0.1f)
    {
        StartCoroutine(FadeTo(length, 0, sound));
    }
    
    public IEnumerator FadeTo(float duration, float targetVolume, Sound audioSource=null)
    {
        float currentTime = 0;
        float start = audioSource.source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.source.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        audioSource.source.volume = targetVolume;

        if (audioSource.source.volume <= 0.1)
        {
            Stop(audioSource);
        }
    }
}
