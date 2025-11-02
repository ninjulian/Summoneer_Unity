using UnityEngine;
using DG.Tweening;

public class GameInitiation : MonoBehaviour
{
    // [SerializeField] private GameObject pixelTexture;

    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerStats playerStats;

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerStats = player.GetComponent<PlayerStats>();

        DOTween.useSafeMode = true;
    }
}
