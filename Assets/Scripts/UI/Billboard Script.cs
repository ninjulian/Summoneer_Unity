using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    //Simple Code to make anything face the camera at all times

    // Update is called once per frame
    void FixedUpdate()
    {
        // Face the camera
        transform.forward = Camera.main.transform.forward;
    }
}
