using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject menu;

    public void OpenShop()
    {
        menu.SetActive(true);
    }

    public void CloseShop()
    {
        menu.SetActive(false);
    }
}
