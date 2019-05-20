using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlightHUDController : MonoBehaviour
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

    private FlightModel flightModel;

    private void Awake()
    {
        flightModel = GetComponentInParent<FlightModel>();
    }

    private void Update()
    {
        UpdateHealthStats();
        UpdateWeaponStats();
    }

    void UpdateHealthStats()
    {
        float health = flightModel.health.health;
        healthUtils.healthText.text = Mathf.RoundToInt(health).ToString();
        healthUtils.healthSliderImage.fillAmount = HelperUtilities.Remap01(health, 0, flightModel.health.maxhealth);
    }

    void UpdateWeaponStats()
    {
        RangeWeapon rangeWeapon = flightModel.flightAvatar.flightWeapon;
        weaponUtils.roundCountText.text =
            "/" + (rangeWeapon.roundsLeft < 10 ? "0" : "") + rangeWeapon.roundsLeft;
        weaponUtils.bulletCountText.text = rangeWeapon.bulletsInCurrentRound.ToString();
    }

    public void Show(bool show = true)
    {
        gameObject.SetActive(show);
    }
}