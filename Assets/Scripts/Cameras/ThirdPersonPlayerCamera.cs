using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonPlayerCamera : StatefulCinemachineCamera
{
    public float followSpeed = 3f;
    public float rotationSpeed = 3f;

    public float minAngle = -60f;
    public float maxAngle = 60f;

    private Transform target;

    #region Accessors

    #endregion

    private CameraInputController _cameraInputController;

    new void Awake()
    {
        base.Awake();
        _cameraInputController = GetComponentInChildren<CameraInputController>();
    }

    protected override void OnActivated()
    {
        
    }

    protected override void OnDeactivated()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        target = LevelManager.Instance.LocalPlayerModel.thirdPersonCamTarget;

//        HelperUtilities.UpdateCursorLock(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePosition();
        UpdateRotation();
    }

    void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, target.position,
            followSpeed * Time.fixedDeltaTime);
    }

    void UpdateRotation()
    {
        CameraInputController.CameraInput playerInput = _cameraInputController.GetCameraInput();

        Vector3 targetRotationAngle = transform.localRotation.eulerAngles;
        targetRotationAngle.x += playerInput.camVertical;
        targetRotationAngle.y += playerInput.camHorizontal;

        targetRotationAngle.x = HelperUtilities.ClampAngle(targetRotationAngle.x, minAngle, maxAngle);
        targetRotationAngle.y = Mathf.Repeat(targetRotationAngle.y, 360f);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotationAngle),
            Time.fixedDeltaTime * rotationSpeed);

//        transform.Rotate(playerInput.camVertical, playerInput.camHorizontal, 0);

        VirtualCamera.transform.LookAt(target.position);
    }
}