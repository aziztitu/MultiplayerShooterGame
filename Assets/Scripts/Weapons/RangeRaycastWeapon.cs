using System;
using UnityEngine;

public class RangeRaycastWeapon : RangeWeapon
{
    public override Type InfoAssetType => typeof(RangeRaycastWeaponInfoAsset);
    public new RangeRaycastWeaponInfoAsset weaponInfoAsset => GetWeaponInfoAsset<RangeRaycastWeaponInfoAsset>();

    protected override void OnShotFired()
    {
        Transform src = aimSource ? aimSource : muzzle;
        
        RaycastHit hitInfo;
        if (Physics.Raycast(src.position, src.forward, out hitInfo, weaponInfoAsset.maxRange))
        {
            if (weaponInfoAsset.hitEffectPrefab)
            {
                GameObject hitEffect = Instantiate(weaponInfoAsset.hitEffectPrefab, hitInfo.point, Quaternion.identity);
                Destroy(hitEffect, weaponInfoAsset.hitEffectDuration);
            }

            OnHitObject(hitInfo);
        }
    }

    protected void OnHitObject(RaycastHit raycastHit)
    {
        Debug.Log("Hit Object: " + raycastHit.transform.gameObject.name);

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
            shootable.OnShot(weaponInfoAsset.damage, raycastHit.point);
        }
    }
}