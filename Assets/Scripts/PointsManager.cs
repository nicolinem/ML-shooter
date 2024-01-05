using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// TODO: Fix current health and shots

public class PointsManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI healthScore;
    [SerializeField] public TextMeshProUGUI keysScore;
    [SerializeField] public TextMeshProUGUI shotsScore;

    int keys = 0;
    int maxKey = 3;
    int shots = 10;



    void Start()
    {

        keysScore.text = keys.ToString() + "/" + maxKey.ToString();
        shotsScore.text = shots.ToString();

        PlayerItems.OnKeyCollected += UpdateKeysUI;

    }

    void Update()
    {

    }

    // Update the keys UI when a key is collected

    public void UpdateHealthUI(int currentHealth)
    {
        healthScore.text = currentHealth.ToString();
    }


    void UpdateKeysUI()
    {
        keys++;
        keysScore.text = keys.ToString() + "/" + maxKey.ToString();
    }
}
