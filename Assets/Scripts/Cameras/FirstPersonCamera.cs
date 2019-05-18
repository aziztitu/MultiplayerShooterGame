using Cinemachine;
using UnityEngine;

public class FirstPersonCamera : StatefulCinemachineCamera
{
    public float camRotationSpeed;
    public float minCamAngle;
    public float maxCamAngle;

    private CameraInputController _cameraInputController;
    private Animator _animator;

    new void Awake()
    {
        base.Awake();
        _cameraInputController = GetComponent<CameraInputController>();
        _animator = GetComponent<Animator>();
    }

    protected override void OnActivated()
    {
        if (LevelManager.Instance.LocalPlayerModel)
        {
            VirtualCamera.Follow = LevelManager.Instance.firstPersonCameraFollow;
        }

        LevelManager.Instance.OnLocalPlayerModelChanged += () =>
        {
            VirtualCamera.Follow = LevelManager.Instance.firstPersonCameraFollow;
        };

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

    private void Update()
    {
        CameraInputController.CameraInput cameraInput = _cameraInputController.GetCameraInput();
        UpdateForCombat(cameraInput);
    }

    private void FixedUpdate()
    {
        CameraInputController.CameraInput cameraInput = _cameraInputController.GetCameraInput();
        RotateCamera(cameraInput);
    }

    void RotateCamera(CameraInputController.CameraInput cameraInput)
    {
        Vector3 targetRotationAngle = transform.localRotation.eulerAngles;
        targetRotationAngle.x += cameraInput.camVertical;
        targetRotationAngle.y += cameraInput.camHorizontal;

        targetRotationAngle.x = HelperUtilities.ClampAngle(targetRotationAngle.x, minCamAngle, maxCamAngle);
        targetRotationAngle.y = Mathf.Repeat(targetRotationAngle.y, 360f);
        targetRotationAngle.z = 0;

        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(targetRotationAngle),
            Time.fixedDeltaTime * camRotationSpeed);
    }

    void UpdateForCombat(CameraInputController.CameraInput cameraInput)
    {
        _animator.SetBool("isAiming", cameraInput.aim);
    }
}