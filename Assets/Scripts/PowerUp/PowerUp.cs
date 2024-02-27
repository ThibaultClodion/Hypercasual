using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PowerUp : ScriptableObject
{
    //Display shop Infos
    public string[] descriptions;
    public int[] price;

    //Player Prefs
    public float[] PlayersPrefSet;
    public string playerPrefsName;
    public bool playerPrefsIsInt;
}
