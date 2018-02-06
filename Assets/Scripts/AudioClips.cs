using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

[Serializable]
public class AudioClips : ScriptableObject {

    [Serializable]
    public class Clip
    {
        public AudioClip audioClip;
        [Range(0, 1f)]
        public float volume;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/AudioClips")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<AudioClips>();
    }
#endif

    [SerializeField]
    public List<Clip> audioClips = new List<Clip>();

    private int lastClip;

    public void ResetLastClipIndex()
    {
        lastClip = 0;
    }

    public Clip NextClip()
    {
        if (lastClip == audioClips.Count)
        {
            lastClip = 0;
        }
        return audioClips[lastClip++];
    }


}
