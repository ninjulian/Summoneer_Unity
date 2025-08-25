using UnityEngine;

public class GameInitiation : MonoBehaviour
{
    [SerializeField] private GameObject pixelTexture;

    private void Awake()
    {
        pixelTexture.SetActive(true);
    }
}
