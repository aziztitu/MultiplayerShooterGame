using System;
using UnityEngine;

public abstract class Projectile: Bolt.EntityBehaviour<IProjectileState>
{
    protected float range;

    public ProjectileInfoAsset projectileInfoAsset;
    public abstract Type InfoAssetType { get; }

    protected IWeaponOwner launchedBy = null;

    public U GetProjectileInfoAsset<U>() where U : ProjectileInfoAsset
    {
        return projectileInfoAsset as U;
    }
    
    public U GetState<U>() where U : IProjectileState
    {
        return (U) state;
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

    public override void Attached()
    {
        base.Attached();

        if (entity.IsOwner)
        {
            SetupState();
        }
    }

    void SetupState()
    {
        state.SetTransforms(state.ProjectileTransform, transform);
    }

    public void SetBulletRange(float bulletRange)
    {
        range = bulletRange;
    }

    protected bool IsShootable(Collider other, out Shootable shootable)
    {
        shootable = other.GetComponent<Shootable>();
        if (!shootable)
        {
            if (other.attachedRigidbody)
            {
                shootable = other.attachedRigidbody.GetComponent<Shootable>();
            }
        }

        if (shootable)
        {
            return true;
        }

        return false;
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

    public void Launch(Vector3 direction, IWeaponOwner weaponOwner)
    {
        launchedBy = weaponOwner;
        OnLaunched(direction);
    }

    protected abstract void OnLaunched(Vector3 direction);
}