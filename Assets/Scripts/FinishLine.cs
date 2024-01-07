using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision){
        if(collision.tag == "Player"){
            SceneManager.LoadScene("FinishMenu");
        }
    }
}
