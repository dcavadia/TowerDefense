using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoPanel : MonoBehaviour
{
    public TMP_Text HealthInfo;
    public TMP_Text CoinsInfo;

    // Update is called once per frame
    void Update()
    {
        HealthInfo.text = "Health: " + PlayerManager.Instance.GetPlayerHealth().ToString();
        CoinsInfo.text = "Coins: " + EconomyManager.Instance.GetPlayerCoins().ToString();
    }
}
