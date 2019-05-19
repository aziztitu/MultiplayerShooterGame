using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected float range;

    public ProjectileInfoAsset projectileInfoAsset;
    public abstract Type InfoAssetType { get; }

    public T GetProjectileInfoAsset<T>() where T : ProjectileInfoAsset
    {
        return projectileInfoAsset as T;
    }

    private void Awake()
    {
        range = projectileInfoAsset.defaultRange;
    }

    private void OnValidate()
    {
        if (projectileInfoAsset)
        {
            if (projectileInfoAsset.GetType() != InfoAssetType)
            {
                projectileInfoAsset = null;
                Debug.LogError("Please use a projectile info asset of type: " + InfoAssetType.Name);
            }
        }
    }
    
    public void SetBulletRange(float bulletRange)
    {
        range = bulletRange;
    }

    protected bool IsShootable(Collision other, out Shootable shootable)
    {
        shootable = other.collider.GetComponent<Shootable>();
        if (!shootable)
        {
            if (other.rigidbody)
            {
                shootable = other.rigidbody.GetComponent<Shootable>();
            }
        }

        if (shootable)
        {
            return true;
        }

        return false;
    }

    public abstract void Launch(Vector3 direction);
}