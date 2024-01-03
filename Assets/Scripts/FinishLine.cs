using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private AudioManager audioManager;

    private void Awake(){
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider collision){
        if(collision.tag == "Player"){
            SceneManager.LoadScene("FinishMenu");
            audioManager.PlaySFX(audioManager.finish);
            Debug.Log("Finish");
        }
    }
}
