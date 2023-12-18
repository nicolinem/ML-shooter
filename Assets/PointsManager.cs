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

    public Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();

        if (player != null)
        {
            healthScore.text = player.CurrentHealth.ToString();
        }

        keysScore.text = keys.ToString() + "/" + maxKey.ToString();
        shotsScore.text = shots.ToString();

        PlayerItems.OnKeyCollected += UpdateKeysUI;
    }

    void Update()
    {
        if (player != null)
        {
            healthScore.text = player.CurrentHealth.ToString();
        }
    }

    // Update the keys UI when a key is collected
    void UpdateKeysUI()
    {
        keys++;
        keysScore.text = keys.ToString() + "/" + maxKey.ToString();
    }
}
