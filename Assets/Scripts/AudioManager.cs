using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;

    public AudioClip key;
    public AudioClip ammo;
    public AudioClip unlock;
    public AudioClip shooting;
    public AudioClip click;

    public AudioClip hurted;
    public AudioClip enemyShooting;

    public AudioClip ghostDying;


    public void PlaySFX(AudioClip audioClip)
    {
        SFXSource.PlayOneShot(audioClip);
    }
}
