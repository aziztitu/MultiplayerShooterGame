using System;
using UnityEngine;

public class RangeProjectileWeapon : RangeWeapon
{
    public GameObject projectilePrefab;

    public override Type InfoAssetType => typeof(RangeProjectileWeaponInfoAsset);
    public new RangeProjectileWeaponInfoAsset weaponInfoAsset => GetWeaponInfoAsset<RangeProjectileWeaponInfoAsset>();

    protected override void OnShotFired(out int bulletsUsed)
    {
        bulletsUsed = 0;
        foreach (var muzzle in muzzles)
        {
            int bulletsLeft = bulletsInCurrentRound - bulletsUsed;
            if (bulletsLeft <= 0)
            {
                return;
            }

            if (ShootFromMuzzle(muzzle))
            {
                bulletsUsed++;
            }
        }
    }

    bool ShootFromMuzzle(Transform muzzle)
    {
        Vector3 shootDir;
        if (aimSource)
        {
            Vector3 hitTargetPos;

            int layerMask = -5;
            layerMask &= ~(1 << LayerMask.NameToLayer("LocalPlayer"));

            RaycastHit hitInfo;
            if (Physics.Raycast(aimSource.position, aimSource.forward, out hitInfo, weaponInfoAsset.maxRange, layerMask))
            {
                hitTargetPos = hitInfo.point;
            }
            else
            {
                hitTargetPos = aimSource.position + (aimSource.forward * weaponInfoAsset.maxRange);
            }

            if (muzzlePivot)
            {
                hitTargetPos += (muzzle.position - muzzlePivot.position);
            }

            shootDir = hitTargetPos - muzzle.position;
        }
        else
        {
            shootDir = muzzle.forward;
        }

        shootDir.Normalize();
        

        if (Vector3.Angle(muzzle.forward, shootDir) >= 90f)
        {
            return false;
        }
        
//        Debug.Log("Angle: " + Vector3.Angle(muzzle.forward, shootDir));

        BoltEntity projectileEntity =
            BoltNetwork.Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        Projectile projectile = projectileEntity.GetComponent<Projectile>();
        projectile.SetBulletRange(weaponInfoAsset.maxRange);
        projectile.Launch(shootDir, weaponOwner);

        return true;
    }
}