// AudioManager made by James Shipp
// Last updated 9/26/23

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * This script is set up as a singleton, so it's accesible through
 * code anywhere in the project! Put this script on a gameobject in
 * the scene and configure any sound or music you want
 * in the inspector. Then trigger a sound like so:
 * 
 *         AudioManager.Instance().PlaySound("soundName");
 *         
 * Similarly, play a song like this:
 * 
 *         AudioManager.Instance().PlayMusic("songName");
 *         
 * The manager will automatically fade songs in and out when you 
 * select a new one. To fade the current song out without playing
 * a new one, just pass an empty string to PlayMusic():
 * 
 *         AudioManager.Instance().PlayMusic("");
 */
public class AudioManager : MonoBehaviour
{
    public List<Sound> sounds;
    public List<Music> music;
    private List<AudioSource> sources;
    private AudioSource musicSource;

    // singleton
    private static AudioManager instance;

    // tracking music fade
    bool musicFadingIn;
    bool musicFadingout;
    Music queuedClip;
    float fadeCounter;
    private UnityEvent OnFadeCompletion;

    private void Awake()
    {
        // setting up singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        sources = new List<AudioSource>();
        OnFadeCompletion = new UnityEvent();

        musicSource.volume = 0.0f;
        musicSource.loop = true;

        musicFadingIn = false;
        musicFadingout = false;
    }

    public static AudioManager Instance()
    {
        return instance;
    }

    private void Update()
    {
        // music fade in / out
        if (musicFadingIn)
        {
            musicSource.volume += Time.deltaTime;
            if (musicSource.volume >= 1f)
                musicFadingIn = false;
        }
        else if (musicFadingout)
        {
            musicSource.volume -= Time.deltaTime;
            if (musicSource.volume <= 0.0f)
            {
                musicSource.clip = null;
                OnFadeCompletion.Invoke();
                musicFadingout = false;
            }
        }
    }

    public void PlayMusic(string musicClipName)
    {
        // if no music specified, fade out current music
        if (musicClipName == "")
        {
            MusicFadeOut(false);
            return;
        }

        // find music
        Music musicClip = music.Find(x => x.name == musicClipName);
        if (musicClip == null)
            throw new Exception("Tried to play music " + musicClipName + ", music doesn't exist in audio manager list");

        // already playing this song
        if (musicClip.clip == musicSource.clip)
            return;

        // set new music!
        queuedClip = musicClip;
        if (musicSource.volume <= 0.0f)
            MusicFadeIn();
        else
            MusicFadeOut(true);
    }

    private void MusicFadeIn()
    {
        musicSource.clip = queuedClip.clip;
        musicSource.Play();
        musicFadingIn = true;
        OnFadeCompletion.RemoveListener(MusicFadeIn);
    }

    private void MusicFadeOut(bool fadeIntoNewSong)
    {
        musicFadingout = true;

        if (fadeIntoNewSong)
            OnFadeCompletion.AddListener(MusicFadeIn);
    }

    public void PlaySound(string soundName)
    {
        AudioSource selectedSource = null;

        // search existing sources for a free one
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
            {
                selectedSource = source;
                break;
            }
        }

        // no audio sources available, so make a new one
        if (selectedSource == null)
        {
            selectedSource = gameObject.AddComponent<AudioSource>();
            sources.Add(selectedSource);
        }

        Sound currentSound = sounds.Find(x => x.name == soundName);

        if (currentSound == null)
            throw new Exception("Tried to play sound " + soundName + ", sound doesn't exist in audio manager list");

        selectedSource.volume = UnityEngine.Random.Range(currentSound.baseVolume - currentSound.volumeVariationLow,
            currentSound.baseVolume + currentSound.volumeVariationHigh);
        selectedSource.pitch = UnityEngine.Random.Range(currentSound.basePitch - currentSound.pitchVariationLow,
            currentSound.basePitch + currentSound.pitchVariationHigh);
        selectedSource.clip = currentSound.clip;
        selectedSource.Play();
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    // volume settings
    public float volumeVariationLow = 0.0f;
    public float volumeVariationHigh = 0.0f;
    public float baseVolume = 1.0f;
    // pitch settings
    public float pitchVariationLow = 0.0f;
    public float pitchVariationHigh = 0.0f;
    public float basePitch = 1.0f;
}

[System.Serializable]
public class Music
{
    public string name;
    public AudioClip clip;
}