using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemText : MonoBehaviour
{
    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Gems").ToString();
    }
}
