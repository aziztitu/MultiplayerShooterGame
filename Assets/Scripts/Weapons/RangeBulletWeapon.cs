using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Info/Range Weapon (Bullet)", fileName = "Range Bullet Weapon")]
public class RangeBulletWeaponInfoAsset: RangeWeaponInfoAssetBase
{
    public BulletInfoAsset bulletInfoAsset;
}

public class RangeBulletWeapon: RangeWeaponBase<RangeBulletWeaponInfoAsset>
{
        
}