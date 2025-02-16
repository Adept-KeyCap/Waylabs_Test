using UnityEngine;


public class FrameRateLimit : MonoBehaviour
{
    [SerializeField] private int targetFPS = 60; // Change this value to set the FPS limit

    void Start()
    {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0; // Disable V-Sync to enforce the frame rate cap

        Object.DontDestroyOnLoad(this);

    }
}
