using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class WeaponPortal : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Material playerMaterial;

    //Which weapon it is
    private bool isRed = false;
    private bool isGreen = false;
    private bool isYellow = false;

    // Start is called before the first frame update
    void Start()
    {
        if (weaponData.name.Contains("Red"))
        {
            isRed = true;
        }
        else if(weaponData.name.Contains("Green"))
        {
            isGreen = true;
        }
        else if(weaponData.name.Contains("Yellow"))
        {
            isYellow = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(isRed)
            {
                GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().InitializeShoot(weaponData, PlayerPrefs.GetInt("Upgrade_redWeaponIndex"), other.GetComponent<Character>(), playerMaterial);
            }
            else if (isGreen)
            {
                GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().InitializeShoot(weaponData, PlayerPrefs.GetInt("Upgrade_greenWeaponIndex"), other.GetComponent<Character>(), playerMaterial);
            }
            else if(isYellow)
            {
                GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().InitializeShoot(weaponData, PlayerPrefs.GetInt("Upgrade_yellowWeaponIndex"), other.GetComponent<Character>(), playerMaterial);
            }
        }
    }
}
