using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDictionary", menuName = "AudioDictionary/new AudioDirctionary")]
public class AudioDictionary : ScriptableObject
{
    [System.Serializable]
    public class AudioGroup
    {
        public string audioName;
        public AudioClip[] audioClips;
    }

    public AudioGroup[] audioGroups;

    Dictionary<string, AudioClip[]> audioDictionary = new Dictionary<string, AudioClip[]>();

    public void Init()
    {
        foreach(AudioGroup audioGroup in audioGroups)
        {
            audioDictionary[audioGroup.audioName] = audioGroup.audioClips;
        }
    }

    public AudioClip GetAudioClipByName(string audioName)
    {
        AudioClip[] audioClips;
        if (audioDictionary.TryGetValue(audioName, out audioClips))
        {
            return audioClips[(int)Random.Range(0, audioClips.Length)];
        }

        return null;
    }
}
