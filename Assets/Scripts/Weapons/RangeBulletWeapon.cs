using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Info/Range Weapon (Bullet)", fileName = "Range Bullet Weapon")]
public class RangeBulletWeaponInfoAsset: RangeWeaponInfoAsset
{
    public BulletInfoAsset bulletInfoAsset;
}

public class RangeBulletWeapon: RangeWeapon
{
    public override Type InfoAssetType => typeof(RangeBulletWeaponInfoAsset);
    protected override void OnShotFired()
    {
        
    }
}