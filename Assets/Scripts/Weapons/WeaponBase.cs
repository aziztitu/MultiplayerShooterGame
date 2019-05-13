using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInfoAssetBase: ScriptableObject
{
    /*public enum WeaponType
    {
        Melee,
        Range
    }
    
    protected WeaponType weaponType;*/

    public GameObject weaponPrefab;
}

public abstract class WeaponBase<T> : MonoBehaviour where T: WeaponInfoAssetBase
{
    public T weaponInfoAsset;
    
    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }
}
