using UnityEngine;
public class CameraReferences : MonoBehaviour
{
    public static CameraReferences Instance { get; private set; } // Singleton

    public Camera playerCamera;
    public Transform aimObject;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
