using System;
using System.Collections;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public AudioClip dmg;
    public AudioClip heal;
    public AudioClip attack;
    public AudioClip getHit;
    public AudioClip menu;

    private uint actualTheme = 0;
    private uint actualLoopId = 0;
    private AudioSource audioSource;
    private static SoundManager _instance;
    public static SoundManager instance
    {
        get
        {
            return _instance;
        }
    }

    [Serializable]
    public class Theme
    {
        public AudioClip[] clip;
    }
    [SerializeField] public Theme[] Themes;

    void Awake()
    {
        _instance = this;
        audioSource = GetComponent<AudioSource>();
        actualTheme = (uint)Themes.Length - 2;
    }
    private void Start()
    {
        PlaySound(gameObject, menu);
    }
    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            if (++actualLoopId >= Themes[actualTheme].clip.Length)
                actualLoopId = 0;

            audioSource.clip = Themes[actualTheme].clip[actualLoopId];
            audioSource.Play();
        }
    }
    public void PlayDmg(GameObject givenGameObject)
    {
        PlaySound(givenGameObject, dmg);
    }
    public void PlayAttack(GameObject givenGameObject)
    {
        PlaySound(givenGameObject, attack);
    }  
    public void PlayHeal(GameObject givenGameObject)
    {
        PlaySound(givenGameObject, heal);
    }
    public void PlayGetHit(GameObject givenGameObject)
    {
        PlaySound(givenGameObject, getHit);
    }

    private void PlaySound(GameObject givenGameObject, AudioClip audioClip)
    {
        AudioSource actualAudioSource = givenGameObject.GetComponent<AudioSource>();
        actualAudioSource.PlayOneShot(audioClip);
    }

    public void ChangeTheme(uint id)
    {
        if (id >= Themes.Length || id < 0 || id == actualTheme)
            return;

        actualLoopId = uint.MaxValue;
        actualTheme = id;
    }
}
