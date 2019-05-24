using System;
using UnityEngine;

public class RangeRaycastWeapon : RangeWeapon
{
    public override Type InfoAssetType => typeof(RangeRaycastWeaponInfoAsset);
    public new RangeRaycastWeaponInfoAsset weaponInfoAsset => GetWeaponInfoAsset<RangeRaycastWeaponInfoAsset>();

    protected override void OnShotFired(out int bulletsUsed)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(aimSource.position, aimSource.forward, out hitInfo, weaponInfoAsset.maxRange))
        {
            if (weaponInfoAsset.hitEffectPrefab)
            {
                GameObject hitEffect = Instantiate(weaponInfoAsset.hitEffectPrefab, hitInfo.point, Quaternion.identity);
                Destroy(hitEffect, weaponInfoAsset.hitEffectDuration);
            }

            OnHitObject(hitInfo);
        }
        
        bulletsUsed = 1;
    }

    protected void OnHitObject(RaycastHit raycastHit)
    {
        Shootable shootable = raycastHit.collider.GetComponent<Shootable>();
        if (!shootable)
        {
            if (raycastHit.rigidbody)
            {
                shootable = raycastHit.rigidbody.GetComponent<Shootable>();
            }
        }
        
        if (shootable)
        {
            Debug.Log("Hit Object: " + raycastHit.transform.gameObject.name);
            shootable.OnShot(weaponInfoAsset.damage, raycastHit.point, weaponOwner);
        }
    }
}