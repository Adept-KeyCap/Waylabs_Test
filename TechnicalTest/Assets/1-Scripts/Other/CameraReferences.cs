using UnityEngine;
public class CameraReferences : MonoBehaviour
{
    public static CameraReferences Instance { get; private set; } // Singleton

    public Camera playerCamera;
    public Transform aimObject;
    public GameObject mainCrosshair;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
