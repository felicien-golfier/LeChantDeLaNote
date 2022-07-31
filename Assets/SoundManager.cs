using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public AudioClip dmg;
    public AudioClip heal;
    public AudioClip attack;

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
        if (actualAudioSource == null)
            actualAudioSource = audioSource;
        actualAudioSource.PlayOneShot(audioClip);
    }
}
