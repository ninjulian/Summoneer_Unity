using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [Header("Crosshair Images")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Sprite crosshair1;
    [SerializeField] private Sprite crosshair2;
    [SerializeField] private Sprite crosshair3;

    public enum CrosshairType { Shooting1, Shooting2, Default }
    public CrosshairType currentCrosshairType;

    // Changes crosshair type
    public void Update()
    {
        switch (currentCrosshairType)
        {
            case CrosshairType.Shooting1:
                crosshair.sprite = crosshair1;
                break;

            case CrosshairType.Shooting2:
                crosshair.sprite = crosshair2;

                break;

            default:
                crosshair.sprite = crosshair3;
                break;

        }
    }


}
