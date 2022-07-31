using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public AudioClip dmg;
    public AudioClip heal;
    public AudioClip attack;

    public AudioClip[][] Themes;

    private uint actualTheme = 0;
    private AudioSource audioSource;
    private static SoundManager _instance;
    public static SoundManager instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        ChangeTheme(0);
    }
    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = Themes[actualTheme][Random.Range(0, 3)];
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

    private void PlaySound(GameObject givenGameObject, AudioClip audioClip)
    {
        AudioSource actualAudioSource = givenGameObject.GetComponent<AudioSource>();
        actualAudioSource.PlayOneShot(audioClip);
    }

    public void ChangeTheme(uint id)
    {
        actualTheme = id;
    }
}
