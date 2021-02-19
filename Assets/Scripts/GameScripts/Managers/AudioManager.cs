using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public const int WeaponCount = 3;
    private static AudioManager instance;
    public static AudioManager Instance { get => instance; set => instance = value; }

    [Header("环境声音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;

    [Header("玩家声音")]
    public AudioClip hurtClip;
    public AudioClip weaponClip;
    public AudioClip lowerLimb;

    [Header("敌人声音")]
    public AudioClip bugHurtClip;
    public AudioClip bugDeathClip;
    public AudioClip houndDeathClip;
    public AudioClip bugGunnerHurtClip;
    public AudioClip bugGunnerDeathClip;
    public AudioClip bugBuilderHurtClip;
    public AudioClip bugBuilderDeathClip;
    [Header("特效声音")]
    public AudioClip coinClip;

    [Header("武器音效")]
    public AudioClip[] shootClip  = new AudioClip[WeaponCount];
    public AudioClip[] reloadClip = new AudioClip[WeaponCount];
    public AudioClip noBullet;


    public AudioSource ambientSource;
    public AudioSource musicSource;
    public static AudioSource voiceSource;
    public AudioSource effectSource;
    public static AudioSource outSideAudioNoExclusionSource;
    public static AudioSource outSideAudioExclusionSource;

    private void Awake()
    {
        Instance = this;
        
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();
        effectSource = gameObject.AddComponent<AudioSource>();
        outSideAudioNoExclusionSource = gameObject.AddComponent<AudioSource>();
        outSideAudioExclusionSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public static void PlayClip(AudioClip audioClip)
    {
        outSideAudioNoExclusionSource.PlayOneShot(audioClip);
    }

    //排斥性音源
    public static void PlayEClip(AudioClip audioClip)
    {
        outSideAudioExclusionSource.clip = audioClip;
        outSideAudioExclusionSource.Play();
    }
}
