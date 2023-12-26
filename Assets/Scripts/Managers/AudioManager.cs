using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public enum AudioChannel { Master, Sfx, Music };

    public float masterVolumePercent { get; private set; } = .3f;
    public float sfxVolumePercent { get; private set; } = 1;
    public float musicVolumePercent { get; private set; } = .5f;

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    Transform audioListenerT;
    Transform playerT;

    public AudioDictionary audioDictionary;

    protected override void Awake()
    {
        base.Awake();

        masterVolumePercent = PlayerPrefs.GetFloat("MasterVolume", masterVolumePercent);
        sfxVolumePercent = PlayerPrefs.GetFloat("SfxVolume", sfxVolumePercent);
        musicVolumePercent = PlayerPrefs.GetFloat("musicVolume", musicVolumePercent);
        musicSources = new AudioSource[2];

        GameObject new2DSfxSource = new GameObject("2D Sfx Source");
        sfx2DSource = new2DSfxSource.AddComponent<AudioSource>();
        sfx2DSource.transform.parent = transform;
        for(int i = 0; i < musicSources.Length; i++)
        {
            GameObject newMusicSource = new GameObject(string.Format("Music source {0}", i + 1));
            newMusicSource.transform.parent = transform;
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
        }

        GameObject audioListener = new GameObject("Audio Listener");
        audioListener.AddComponent<AudioListener>();
        audioListener.transform.parent = transform;

        audioListenerT = audioListener.transform;
        Player player = FindObjectOfType<Player>();
        if (player)
            playerT = player.transform;
    }

    private void Update()
    {
        if (playerT && playerT.gameObject.active)
            audioListenerT.position = playerT.position;
    }

    public void SetAudioDict(AudioDictionary audioDictionary)
    {
        this.audioDictionary = audioDictionary;
        if (this.audioDictionary != null)
            this.audioDictionary.Init();
    }

    public void SetVolume(AudioChannel channel, float volume)
    {
        switch (channel)
        {
            case (AudioChannel.Master):
                masterVolumePercent = volume;
                break;
            case (AudioChannel.Sfx):
                sfxVolumePercent = volume;
                break;
            case (AudioChannel.Music):
                musicVolumePercent = volume;
                break;
        }

        musicSources[activeMusicSourceIndex].volume = masterVolumePercent * musicVolumePercent;

        PlayerPrefs.SetFloat("MasterVolume", masterVolumePercent);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolumePercent);
        PlayerPrefs.SetFloat("musicVolume", musicVolumePercent);
        PlayerPrefs.Save();
    }

    IEnumerator AnimateMusicCrossFade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, masterVolumePercent * musicVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(masterVolumePercent * musicVolumePercent, 0, percent);

            //Debug.Log(string.Format("audio {0}, volume: {1}", activeMusicSourceIndex, musicSources[activeMusicSourceIndex].volume));
            //Debug.Log(string.Format("audio {0}, volume: {1}",  1 - activeMusicSourceIndex, musicSources[1 - activeMusicSourceIndex].volume));

            yield return null;
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossFade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string audioName, Vector3 pos)
    {
        AudioClip clip = audioDictionary.GetAudioClipByName(audioName);
        PlaySound(clip, pos);
    }

    public void PlaySound2D(string audioName)
    {
        sfx2DSource.PlayOneShot(audioDictionary.GetAudioClipByName(audioName), sfxVolumePercent * masterVolumePercent);
    }
}
