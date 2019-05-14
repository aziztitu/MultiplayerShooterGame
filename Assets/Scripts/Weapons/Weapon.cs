using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInfoAsset : ScriptableObject
{
    /*public enum WeaponType
    {
        Melee,
        Range
    }
    
    protected WeaponType weaponType;*/

    public GameObject weaponPrefab;
}

public abstract class Weapon : MonoBehaviour
{
    public WeaponInfoAsset weaponInfoAsset;

    public T GetWeaponInfoAsset<T>() where T : WeaponInfoAsset
    {
        return weaponInfoAsset as T;
    }

    // Start is called before the first frame update
    protected void Start()
    {
    }

    // Update is called once per frame
    protected void Update()
    {
    }
}