using FoodyGo.Singletons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pookoomin.Manager
{
    public enum BGM
    {
        Main,
    }
    public enum SFX
    {
        Click, 
        Click2, 
        CameraFlash, 
        DuckToy,
    }

    public class SoundManager : Singleton<SoundManager>
    {
        private AudioSource bgmSource;
        private AudioSource sfxSource;

        private Dictionary<BGM, AudioClip> bgmClips = new Dictionary<BGM, AudioClip>();
        private Dictionary<SFX, AudioClip> sfxClips = new Dictionary<SFX, AudioClip>();

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            GameObject bgmObj = new GameObject("BGM");
            bgmSource = bgmObj.AddComponent<AudioSource>();
            bgmObj.transform.SetParent(transform);

            GameObject sfxObj = new GameObject("SFX");
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxObj.transform.SetParent(transform);

            AudioClip[] BGMClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
            foreach (AudioClip clip in BGMClips)
            {
                try
                {
                    BGM type = (BGM)Enum.Parse(typeof(BGM), clip.name);
                    bgmClips.Add(type, clip);
                }
                catch
                {
                    Debug.LogWarning($"Enum값에 없는 BGM Audio Clip : {clip.name}");
                }
            }
            AudioClip[] SFXClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
            foreach (AudioClip clip in SFXClips)
            {
                try
                {
                    SFX type = (SFX)Enum.Parse(typeof(SFX), clip.name);
                    sfxClips.Add(type, clip);
                }
                catch
                {
                    Debug.LogWarning($"Enum값에 없는 SFX Audio Clip : {clip.name}");
                }
            }

        }

        public void PlayBGM(BGM type)
        {
            if (bgmClips.ContainsKey(type))
            {
                bgmSource.clip = bgmClips[type];
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }

        public void PlaySFX(SFX type)
        {
            if (sfxClips.ContainsKey(type))
            {
                sfxSource.PlayOneShot(sfxClips[type]);
            }
        }
    }
}
