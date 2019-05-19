using System;
using BasicTools.ButtonInspector;
using UnityEngine;

public abstract class RangeWeapon : Weapon
{
    public Transform aimSource;

    public Transform[] muzzles;
    public Transform muzzlePivot;

//    [Button("Shoot", "Shoot")] public bool btn_Shoot;

    [SerializeField] private int _roundsLeft = 5;
    private float nextFireableTime = 0;

    public override Type InfoAssetType => typeof(RangeWeaponInfoAsset);
    public new RangeWeaponInfoAsset weaponInfoAsset => GetWeaponInfoAsset<RangeWeaponInfoAsset>();

    public int roundsLeft
    {
        get { return _roundsLeft; }

        protected set { _roundsLeft = value; }
    }

    public int bulletsInCurrentRound { get; protected set; } = 0;

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
                nextFireableTime = Time.time + (1 / weaponInfoAsset.fireRate);

                int bulletsUsed;
                OnShotFired(out bulletsUsed);
                bulletsInCurrentRound -= bulletsUsed;
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

    protected abstract void OnShotFired(out int bulletsUsed);
}