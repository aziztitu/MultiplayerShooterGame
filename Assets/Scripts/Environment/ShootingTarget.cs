using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    private const float maxHealth = 100;
    
    public Color healthyColor = Color.blue;
    public Color weakColor = Color.red;
    
    public float health = 100;
    
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.blue;
    }

    private void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void OnHit(float damage, Vector3 hitPosition, IWeaponOwner weaponOwner)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        float lerpFactor = (maxHealth - health)/maxHealth;
        meshRenderer.material.color = Color.Lerp(healthyColor, weakColor, lerpFactor);

        if (health <= 0)
        {
            Destroy(this.gameObject);   
        }
    }
}