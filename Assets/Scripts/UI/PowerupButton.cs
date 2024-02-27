using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupButton : MonoBehaviour
{
    [Header("Display GameObject")]
    [SerializeField] GameObject description;
    [SerializeField] Button descriptionButton;
    [SerializeField] private GameObject[] otherDescription;

    [Header("Money text")]
    [SerializeField] MoneyText moneyText;

    [Header("Power up data's")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private PowerUp powerup;

    public void ActivateDescription()
    {
        //Activate the good description Button
        foreach (GameObject description in otherDescription)
        {
            description.SetActive(false);
        }

        description.SetActive(true);

        //Initialize the good text
        UpdateDescriptionText();

        //Activate buy button
        descriptionButton.onClick.AddListener(BuyPowerup);
    }

    public void UpdateDescriptionText()
    {
        //Detect if the powerup is already maxed or not
        if(PlayerPrefs.GetInt(powerup.playerPrefsName + "Index") < powerup.price.Length)
        {
            descriptionText.text = powerup.descriptions[PlayerPrefs.GetInt(powerup.playerPrefsName + "Index")] + " | " + "Price : " + powerup.price[PlayerPrefs.GetInt(powerup.playerPrefsName + "Index")];
        }
        else
        {
            descriptionText.text = "Upgrade is maxed";
        }
    }

    public void BuyPowerup()
    {
        //Get Money and powerup actual Index
        int money = PlayerPrefs.GetInt("Money");
        int index = PlayerPrefs.GetInt(powerup.playerPrefsName + "Index");

        //Check if the powerup is maxed
        if (index < powerup.price.Length)
        {
            //Check if the player has sufficient money
            if (money >= powerup.price[PlayerPrefs.GetInt(powerup.playerPrefsName + "Index")])
            {
                //Update Money information
                PlayerPrefs.SetInt("Money", money - powerup.price[PlayerPrefs.GetInt(powerup.playerPrefsName + "Index")]);
                PlayerPrefs.SetInt(powerup.playerPrefsName + "Index", PlayerPrefs.GetInt(powerup.playerPrefsName + "Index") + 1);
                UpdateDescriptionText();
                moneyText.UpdateText();

                //Apply the powerup
                if(powerup.playerPrefsIsInt)
                {
                    PlayerPrefs.SetInt(powerup.playerPrefsName, (int) powerup.PlayersPrefSet[index]);
                }
                else
                {
                    PlayerPrefs.SetFloat(powerup.playerPrefsName, powerup.PlayersPrefSet[index]);
                }
            }
            else
            {
                Debug.Log("Not enough money !");
            }
        }
        else
        {
            Debug.Log("Already max");
        }
    }
}
