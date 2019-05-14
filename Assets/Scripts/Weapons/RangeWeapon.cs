using System;
using BasicTools.ButtonInspector;
using UnityEngine;

public abstract class RangeWeaponInfoAsset : WeaponInfoAsset
{
    public int bulletsPerRound = 30;
    public float fireRate = 5f;
    public float maxRange = 50f;
}

public abstract class RangeWeapon : Weapon
{
    public Transform aimSource;
    public Transform muzzle;

//    [Button("Shoot", "Shoot")] public bool btn_Shoot;

    public new RangeWeaponInfoAsset weaponInfoAsset => GetWeaponInfoAsset<RangeWeaponInfoAsset>();
    
    [SerializeField]
    protected int roundsLeft = 5;
    protected int bulletsInCurrentRound = 0;

    private float nextFireableTime = 0;

    protected new void Update()
    {
        base.Update();
        /*if (Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }*/
    }

    public bool Shoot()
    {
        if (Time.time >= nextFireableTime)
        {
            if (bulletsInCurrentRound > 0)
            {
                bulletsInCurrentRound--;
                nextFireableTime = Time.time + (1 / weaponInfoAsset.fireRate);
                OnShotFired();
                return true;
            }

            Reload();
        }

        return false;
    }

    public void Reload()
    {
        if (roundsLeft > 0)
        {
            roundsLeft--;
            bulletsInCurrentRound = weaponInfoAsset.bulletsPerRound;
        }
    }

    protected abstract void OnShotFired();
}