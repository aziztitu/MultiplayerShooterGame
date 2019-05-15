using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    [Serializable]
    public class HealthUtils
    {
        public Image healthSliderImage;
        public TextMeshProUGUI healthText;
    }

    [Serializable]
    public class WeaponUtils
    {
        public Image weaponImage;
        public TextMeshProUGUI bulletCountText;
        public TextMeshProUGUI roundCountText;
    }

    public HealthUtils healthUtils;
    public WeaponUtils weaponUtils;

    private PlayerModel playerModel;

    private void Awake()
    {
        playerModel = GetComponentInParent<PlayerModel>();
    }

    private void Update()
    {
        UpdateHealthStats();
        UpdateWeaponStats();
    }

    void UpdateHealthStats()
    {
        float health = playerModel.health.health;
        healthUtils.healthText.text = Mathf.RoundToInt(health).ToString();
        healthUtils.healthSliderImage.fillAmount = HelperUtilities.Remap01(health, 0, playerModel.health.maxhealth);
    }

    void UpdateWeaponStats()
    {
        RangeWeapon rangeWeapon = playerModel.playerCombatController.rangeWeapon;
        weaponUtils.roundCountText.text =
            "/" + (rangeWeapon.roundsLeft < 10 ? "0" : "") + rangeWeapon.roundsLeft;
        weaponUtils.bulletCountText.text = rangeWeapon.bulletsInCurrentRound.ToString();
    }
}