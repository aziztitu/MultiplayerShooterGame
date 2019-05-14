using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Info/Range Weapon (Bullet)", fileName = "Range Bullet Weapon")]
public class RangeBulletWeaponInfoAsset: RangeWeaponInfoAsset
{
    public BulletInfoAsset bulletInfoAsset;
}

public class RangeBulletWeapon: RangeWeapon
{
    protected override void OnShotFired()
    {
        
    }
}