using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ч���������������������������
/// </summary>
public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip jump,magic,switchFood;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJump()
    {
        audioSource.PlayOneShot(jump);
    }

    public void PlayMagic()
    {
        audioSource.PlayOneShot(magic);
    }

    public void PlaySwitchFood()
    {
        audioSource.PlayOneShot(switchFood);
    }
}
