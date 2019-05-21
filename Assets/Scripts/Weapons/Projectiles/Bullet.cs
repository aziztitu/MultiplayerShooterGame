using System;
using UnityEngine;

public class Bullet : Projectile<IBulletState>
{
    private float speed;
    private Vector3 velocity = Vector3.zero;

    private Vector3 launchPos;

    public override Type InfoAssetType => typeof(BulletInfoAsset);
    public new BulletInfoAsset projectileInfoAsset => GetProjectileInfoAsset<BulletInfoAsset>();

    private Rigidbody _rigidbody;

    private void Awake()
    {
        speed = projectileInfoAsset.defaultSpeed;
        launchPos = transform.position;

        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void SimulateOwner()
    {
        base.SimulateOwner();
        
        if (Vector3.Distance(transform.position, launchPos) > range)
        {
            BoltNetwork.Destroy(this.gameObject);
        }
    }

    public void SetBulletSpeed(float bulletSpeed)
    {
        speed = bulletSpeed;
    }

    public override void Launch(Vector3 direction)
    {
        transform.LookAt(transform.position + (direction * 5));
        launchPos = transform.position;
        velocity = direction * speed;

        _rigidbody.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!entity.IsOwner)
        {
            return;
        }
        
        if (other.gameObject.CompareTag("Bullet") ||
            (other.attachedRigidbody && other.attachedRigidbody.gameObject.CompareTag("Bullet")))
        {
            return;
        }

        if (other.attachedRigidbody && other.attachedRigidbody.gameObject.layer == LayerMask.NameToLayer("LocalPlayer"))
        {
            return;
        }

        Debug.Log("Hit Object: " + other.transform.gameObject.name);

        Vector3 hitPos = other.ClosestPoint(transform.position);

        GameObject hitEffect = Instantiate(projectileInfoAsset.hitEffectPrefab, hitPos, Quaternion.identity);
        Destroy(hitEffect, projectileInfoAsset.hitEffectDuration);

        Shootable shootable;
        if (IsShootable(other, out shootable))
        {
            shootable.OnShot(projectileInfoAsset.damage, hitPos);
        }

        BoltNetwork.Destroy(gameObject);
    }

    /*protected void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            return;
        }
        
        Debug.Log("Hit Object: " + other.transform.gameObject.name);
        
        Vector3 hitPos = other.transform.position;
        if (other.contacts.Length > 0)
        {
            hitPos = other.contacts[0].point;
        }

        GameObject hitEffect = Instantiate(projectileInfoAsset.hitEffectPrefab, hitPos, Quaternion.identity);
        Destroy(hitEffect, projectileInfoAsset.hitEffectDuration);

        Shootable shootable;
        if (IsShootable(other, out shootable))
        {
            shootable.OnShot(projectileInfoAsset.damage, hitPos);
        }

        Destroy(gameObject);
    }*/
}