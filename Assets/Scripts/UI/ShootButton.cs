using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootButton : MonoBehaviour
{
    [Header("Backgrounds")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject[] otherBackgrounds;

    [Header("Weapon")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private bool isRed;
    [SerializeField] private bool isGreen;
    [SerializeField] private bool isYellow;


    [Header("Sliders")]
    [SerializeField] private Slider damageSlider;
    [SerializeField] private Slider fireRateSlider;
    [SerializeField] private Slider bulletSpeedSlider;
    [SerializeField] private Slider rangeSlider;

    [Header("Buy Data's")]
    [SerializeField] private GemText gemText;

    private void Start()
    {
        //Update Datas
        UpdateDatas();
    }

    private void UpdateDatas()
    {
        //Initialize Slider Max Values
        damageSlider.maxValue = weaponData.bulletDamage[weaponData.bulletDamage.Length - 1];
        fireRateSlider.maxValue = 1 / weaponData.fireRate[weaponData.fireRate.Length - 1];
        bulletSpeedSlider.maxValue = weaponData.bulletSpeed[weaponData.bulletSpeed.Length - 1];
        rangeSlider.maxValue = weaponData.bulletRange[weaponData.bulletRange.Length - 1];

        //Initialize actual Slider Values
        int weaponIndex = GetWeaponIndex();
        damageSlider.value = weaponData.bulletDamage[weaponIndex];
        fireRateSlider.value = 1 / weaponData.fireRate[weaponIndex];
        bulletSpeedSlider.value = weaponData.bulletSpeed[weaponIndex];
        rangeSlider.value = weaponData.bulletRange[weaponIndex];

        //Update buy text
        gemText.UpdateText();
    }

    private int GetWeaponIndex()
    {
        if (isRed)
        {
            return PlayerPrefs.GetInt("Upgrade_redWeaponIndex");
        }
        else if (isYellow)
        {
            return PlayerPrefs.GetInt("Upgrade_yellowWeaponIndex");
        }
        else
        {
            return PlayerPrefs.GetInt("Upgrade_greenWeaponIndex");
        }
    }

    private void IncreaseWeaponIndex()
    {
        if (isRed)
        {
            PlayerPrefs.SetInt("Upgrade_redWeaponIndex", PlayerPrefs.GetInt("Upgrade_redWeaponIndex") + 1);
        }
        else if (isYellow)
        {
            PlayerPrefs.SetInt("Upgrade_yellowWeaponIndex", PlayerPrefs.GetInt("Upgrade_yellowWeaponIndex") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("Upgrade_greenWeaponIndex", PlayerPrefs.GetInt("Upgrade_greenWeaponIndex") + 1);
        }
    }

    public void BuyWeapon()
    {
        int weaponIndex = GetWeaponIndex();
        int gems = PlayerPrefs.GetInt("Gems");
        
        if(weaponIndex < weaponData.price.Length - 1) 
        {
            if (gems >= weaponData.price[weaponIndex])
            {
                PlayerPrefs.SetInt("Gems", gems - weaponData.price[weaponIndex]);
                IncreaseWeaponIndex();
                UpdateDatas();
            }
            else
            {
                Debug.Log("Not enough gems !");
            }
        }
        else
        {
            Debug.Log("Already max");
        }
    }

    public void DisplayBackground()
    {
        for(int i = 0; i < otherBackgrounds.Length; i++)
        {
            otherBackgrounds[i].SetActive(false);
        }

        background.SetActive(true);
    }
}
