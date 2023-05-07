using UnityEngine;

// Billboard effect on images
public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        // Find the main camera in the scene
        mainCamera = UIManager.Instance.topDownCamera;
    }

    private void LateUpdate()
    {
        // Rotate the health bar image to face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
}
