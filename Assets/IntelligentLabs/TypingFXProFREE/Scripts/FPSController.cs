using UnityEngine;

namespace TypingFXProFREE
{
    public class FPSController : MonoBehaviour
    {
        [Tooltip("Set the target frame rate for the application. Use 0 for unlimited.")]
        public int targetFrameRate = 60; // Default value

        void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            Debug.Log("Target Frame Rate set to: " + Application.targetFrameRate);
        }
    }
}
