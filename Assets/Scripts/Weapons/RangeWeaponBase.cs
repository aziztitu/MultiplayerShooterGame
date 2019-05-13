using UnityEngine;

public abstract class RangeWeaponInfoAssetBase : WeaponInfoAssetBase
{
    public int bulletsPerRound = 30;
    public float fireRate = 5f;
}

public abstract class RangeWeaponBase<T> : WeaponBase<T> where T : RangeWeaponInfoAssetBase
{
    protected int roundsLeft = 5;
    protected int bulletsInCurrentRound = 0;
    
    public void Shoot()
    {
        
    }
}
