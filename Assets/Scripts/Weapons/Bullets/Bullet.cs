using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Bullets/Info/Default", fileName = "Bullet Info")]
public class BulletInfoAsset : ScriptableObject
{
    public float damage = 0;
    public GameObject bulletPrefab;
}

public class BulletCustomizable<T> : MonoBehaviour where T: BulletInfoAsset
{
    public BulletInfoAsset bulletInfoAsset;
}