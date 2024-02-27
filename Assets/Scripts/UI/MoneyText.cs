using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyText : MonoBehaviour
{
    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Money").ToString();
    }
}
