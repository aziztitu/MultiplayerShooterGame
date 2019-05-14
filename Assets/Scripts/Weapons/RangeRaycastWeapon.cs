using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Info/Range Weapon (Raycast)", fileName = "Range Raycast Weapon")]
public class RangeRaycastWeaponInfoAsset : RangeWeaponInfoAsset
{
    public GameObject hitEffectPrefab;
    public float hitEffectDuration;
}

public class RangeRaycastWeapon : RangeWeapon
{
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
        // TODO: TBI
    }
}