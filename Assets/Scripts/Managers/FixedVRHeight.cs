using UnityEngine;

public class FixedVRHeight : MonoBehaviour
{
    [SerializeField] private float fixedHeight = 1.65f;

    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main != null ? Camera.main.transform : null;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main != null ? Camera.main.transform : null;

            if (cameraTransform == null)
            {
                return;
            }
        }

        Vector3 cameraLocalPosition = cameraTransform.localPosition;
        cameraLocalPosition.y = fixedHeight;
        cameraTransform.localPosition = cameraLocalPosition;
    }
}