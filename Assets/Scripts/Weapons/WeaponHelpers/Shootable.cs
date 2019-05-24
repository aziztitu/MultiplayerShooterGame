using System;
using UnityEngine;
using UnityEngine.Events;

public class Shootable : MonoBehaviour
{
    [Serializable]
    public class ShootableEvent: UnityEvent<float, Vector3, IWeaponOwner> {}
    
    public ShootableEvent onShotEvent;

    public void OnShot(float damage, Vector3 shotPosition, IWeaponOwner weaponOwner)
    {
        onShotEvent.Invoke(damage, shotPosition, weaponOwner);
    }
}