using FoodyGo.Singletons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pookoomin.Manager
{
    public enum BGM
    {
        Title,
        Main,
    }
    public enum SFX
    {
        Click, 
        Click2, 
        CameraFlash, 
        SqueakerToy,
    }

    public class SoundManager : Singleton<SoundManager>
    {
        private AudioSource bgmSource;
        private AudioSource sfxSource;

        private Dictionary<BGM, AudioClip> bgmClips = new Dictionary<BGM, AudioClip>();
        private Dictionary<SFX, AudioClip> sfxClips = new Dictionary<SFX, AudioClip>();

        private Coroutine currentBGMCoroutine;

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

        public void PlayBGM(BGM type, float fadeDuration = 0.5f)
        {
            if (bgmClips.ContainsKey(type))
            {
                if (currentBGMCoroutine != null)
                { 
                    StopCoroutine(currentBGMCoroutine);
                }

                currentBGMCoroutine = StartCoroutine(FadeOutBGM(fadeDuration, () =>
                {
                    bgmSource.clip = bgmClips[type];
                    bgmSource.loop = true;
                    bgmSource.Play();
                    currentBGMCoroutine = StartCoroutine(FadeInBGM(fadeDuration)); 
                }));

            }
        }

        public void SetBGMVolume(float volume)
        {
            bgmSource.volume = Mathf.Clamp(volume, 0, 1);
        }

        private IEnumerator FadeOutBGM(float duration, Action onFadeComplete)
        {
            float startVolume = bgmSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }

            bgmSource.volume = 0;  
            onFadeComplete?.Invoke(); 
        }

        private IEnumerator FadeInBGM(float duration)
        {
            float startVolume = 0f;
            bgmSource.volume = 0f;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 1f, t / duration);
                yield return null;
            }

            bgmSource.volume = 1.0f;
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
