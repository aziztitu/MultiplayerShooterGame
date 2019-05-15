using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Info/Range Weapon (Raycast)", fileName = "Range Raycast Weapon")]
public class RangeRaycastWeaponInfoAsset : RangeWeaponInfoAsset
{
    public float damage;
    public GameObject hitEffectPrefab;
    public float hitEffectDuration;
}
