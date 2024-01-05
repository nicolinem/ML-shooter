using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;

    public AudioClip finish;
    public AudioClip pickup;
    public AudioClip shooting;

    public void PlaySFX(AudioClip audioClip){
        SFXSource.PlayOneShot(audioClip);
    }
}
