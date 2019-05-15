using System;
using UnityEngine;

public class RangeBulletWeapon: RangeWeapon
{
    public override Type InfoAssetType => typeof(RangeBulletWeaponInfoAsset);
    protected override void OnShotFired()
    {
        
    }
}