using System;
using UnityEngine;

[RequireComponent(typeof(RangeWeapon))]
public class UseCinemachineCameraAsAimSource: MonoBehaviour
{
    private RangeWeapon rangeWeapon;

    private void Awake()
    {
        rangeWeapon = GetComponent<RangeWeapon>();

        CinemachineCameraManager.AddOnSingletonReadyListener((instance) =>
        {
            Debug.Log(instance);
            RefreshAimSource();
            instance.onCinemachineCameraStateUpdated.AddListener((curState) => { RefreshAimSource(); });
        });
    }

    private void RefreshAimSource()
    {
        if (CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera != null)
        {
            rangeWeapon.aimSource = CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.transform;
        }
    }
}