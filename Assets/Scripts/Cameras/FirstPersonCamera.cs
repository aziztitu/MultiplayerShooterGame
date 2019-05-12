using Cinemachine;
using UnityEngine;

public class FirstPersonCamera : StatefulCinemachineCamera
{
    public float camRotationSpeed;
    public float minCamAngle;
    public float maxCamAngle;

    private CameraInputController _cameraInputController;

    new void Awake()
    {
        base.Awake();
        _cameraInputController = GetComponent<CameraInputController>();
    }

    protected override void OnActivated()
    {
        VirtualCamera.Follow = LevelManager.Instance.LocalPlayerModel.firstPersonCamTransform;
        HelperUtilities.UpdateCursorLock(true);
    }

    protected override void OnDeactivated()
    {
        HelperUtilities.UpdateCursorLock(false);
    }

    new void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        CameraInputController.CameraInput playerInput = _cameraInputController.GetCameraInput();

        Vector3 targetRotationAngle = transform.localRotation.eulerAngles;
        targetRotationAngle.x += playerInput.camVertical;
        targetRotationAngle.y += playerInput.camHorizontal;

        targetRotationAngle.x = HelperUtilities.ClampAngle(targetRotationAngle.x, minCamAngle, maxCamAngle);
        targetRotationAngle.y = Mathf.Repeat(targetRotationAngle.y, 360f);
        targetRotationAngle.z = 0;

        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(targetRotationAngle),
            Time.fixedDeltaTime * camRotationSpeed);
    }
}