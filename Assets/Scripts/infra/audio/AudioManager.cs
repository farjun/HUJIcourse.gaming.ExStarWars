using UnityEngine;
using System.Collections.Generic;
using Infra.Utils;

namespace Infra.Audio {
public class AudioManager : MonoBehaviour {
    private static AudioManager instance;

    public float musicVolume = 1.0f;
    public float masterSoundVolume = 1.0f;
    public AudioClip[] musicClips;

    public int soundSourcesToAddWhenNeeded = 5;
    public int maxSoundSources = 20;

    public AudioSource _soundsSources;
    public AudioSource _musicSources;
    private int soundSourceIndex = 0;
    private List<AudioSource> soundsSources;
    private AudioSource[] musicSources;

    protected void Awake() {
        instance = this;

        soundsSources = new List<AudioSource>(_soundsSources.GetComponents<AudioSource>());
        musicSources = _musicSources.GetComponents<AudioSource>();
        soundSourceIndex = 0;
        DebugUtils.Log(string.Format("Using {0} sound sources and {1} music sources", soundsSources.Count, musicSources.Length));

        foreach (var musicSource in musicSources) {
            musicSource.ignoreListenerVolume = true;
            musicSource.loop = true;
        }
        foreach (var soundsSource in soundsSources) {
            soundsSource.loop = false;
        }

        SetMusicVolume(1);
        SetMasterSoundVolume(masterSoundVolume);
    }

    protected void Start() {
        EnableSound();
        EnableMusic();
    }

    public static void SetSoundState(bool isEnabled) {
        foreach (var soundsSource in instance.soundsSources) {
            soundsSource.mute = !isEnabled;
        }
    }

    public static void EnableSound() {
        SetSoundState(true);
    }

    public static void DisableSound() {
        SetSoundState(false);
    }

    public static void SetMusicState(bool isEnabled) {
        if (isEnabled)
            EnableMusic();
        else
            DisableMusic();
    }

    public static void EnableMusic() {
        UnpauseMusic();
    }

    public static void DisableMusic() {
        PauseMusic();
    }

    public static void UnpauseMusic() {
        foreach (var musicSource in instance.musicSources) {
            musicSource.UnPause();
            musicSource.mute = false;
        }
    }

    public static void PauseMusic() {
        foreach (var musicSource in instance.musicSources) {
            if (musicSource == null) continue;
            musicSource.Pause();
            musicSource.mute = true;
        }
    }

    public static void PlayClip(AdjustableAudioClip clip) {
        PlayClip(clip.clip, clip.volume, clip.stopAt, instance.GetAvailableSoundChannelIndex());
    }

    public int GetAvailableSoundChannelIndex() {
        // Look for the next available sound source.
        for (int i = soundSourceIndex; i < soundsSources.Count; i++) {
            var soundSource = soundsSources[i];
            if (!soundSource.isPlaying) {
                soundSourceIndex = i;
                return soundSourceIndex;
            }
        }
        for (int i = 0; i < soundSourceIndex; i++) {
            var soundSource = soundsSources[i];
            if (!soundSource.isPlaying) {
                soundSourceIndex = i;
                return soundSourceIndex;
            }
        }

        var sourcesToAdd = Mathf.Min(soundSourcesToAddWhenNeeded, maxSoundSources - soundsSources.Count);
        if (sourcesToAdd <= 0) {
            // No available sources. Look for the most completed next source and
            // replace it.
            float maxProgress = 0f;
            for (int j = soundSourceIndex + 1; j <= 5; j++) {
                var i = j % soundsSources.Count;
                var soundSource = soundsSources[i];
                var duration = soundSource.clip == null ? 0f : soundSource.clip.length;
                if (duration <= 0) {
                    soundSourceIndex = i;
                    return soundSourceIndex;
                }
                var progress = soundSource.time / duration;
                if (progress > maxProgress) {
                    maxProgress = progress;
                    soundSourceIndex = i;
                }
            }
            return soundSourceIndex;
        }

        // All sound sources are being used. Create some new sources.
        soundSourceIndex = soundsSources.Count;
        for (int i = 0; i < sourcesToAdd; i++) {
            var newSource = _soundsSources.gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.loop = false;
            soundsSources.Add(newSource);
        }

        return soundSourceIndex;
    }

    public static void PlayClip(AudioClip clip, float volume = 1.0f, float stopAt = 0, int index = 0) {
        var soundSource = instance.soundsSources[index];
        soundSource.clip = clip;
        soundSource.volume = volume;
        soundSource.Play();
        if (stopAt > 0 && stopAt < clip.length) {
            CoroutineUtils.DelaySeconds(instance, soundSource.Stop, stopAt);
        }
    }

    public static void PlayClipAtPoint(AdjustableAudioClip clip, Vector3 position) {
        PlayClipAtPoint(clip.clip, position, clip.volume);
    }

    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1.0f) {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public static void PlayMusic(int musicIndex, int index = 0) {
        AudioClip music = instance.musicClips[musicIndex];
        if (instance.musicSources[index].clip == music) {
            return;
        }
        instance.musicSources[index].clip = music;
        instance.musicSources[index].Play();
    }

    public static void SetMasterSoundVolume(float volume) {
        AudioListener.volume = volume;
    }

    public static void SetMusicVolume(float volume) {
        foreach (var musicSource in instance.musicSources) {
            musicSource.volume = volume * instance.musicVolume;
        }
    }

    public static void SetSoundVolume(float volume) {
        foreach (var soundSource in instance.soundsSources) {
            soundSource.volume = volume;
        }
    }
}
}
