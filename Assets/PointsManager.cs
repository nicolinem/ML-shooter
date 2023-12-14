using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI healthScore;
    [SerializeField] public TextMeshProUGUI keysScore;
    [SerializeField] public TextMeshProUGUI shotsScore;

    int keys = 0;
    int maxKey = 0;
    int shots = 10;

    // Reference to the Player script
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        // Assuming you have a reference to the Player script in the scene
        if (player != null)
        {
            // Initialize the UI with the current health value
            healthScore.text = player.CurrentHealth.ToString();
        }
        keysScore.text = keys.ToString() + "/" + maxKey.ToString();
        shotsScore.text = shots.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the health UI in the Update method if needed
        if (player != null)
        {
            healthScore.text = player.CurrentHealth.ToString();
        }
    }
}
