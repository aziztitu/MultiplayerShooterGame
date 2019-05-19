using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Info/Bullet", fileName = "Bullet")]
public class BulletInfoAsset : ProjectileInfoAsset
{
    public float defaultSpeed = 10f;
    public GameObject hitEffectPrefab;
    public float hitEffectDuration;
}