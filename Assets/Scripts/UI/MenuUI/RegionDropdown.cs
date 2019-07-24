using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;

public class RegionDropdown : MonoBehaviour
{
    string[,] regionCodes = new string [,]
    {
        {"USA, West", "usw"},
        {"Regions - Auto", ""},
        {"USA, East", "us"},
        {"Asia", "asia"},
        {"Australia", "au"},
        {"Canada, East", "cae"},
        {"Europe", "eu"},
        {"India", "in"},
        {"Japan", "jp"},
        {"Russia", "ru"},
        {"Russia, East", "rue"},
        {"South America", "sa"},
        {"South Korea", "kr"}
    };

    void Start()
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        InitializeDropdown(dropdown);
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown.value);
        });
        DropdownValueChanged(0);
    }

    private void InitializeDropdown(TMP_Dropdown dropdown)
    {
        for (int i = 0; i < regionCodes.GetLength(0); i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(regionCodes[i, 0]));
        }
    }

    private void SetRegion(string regionCode = "")
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = regionCode;
    }

    void DropdownValueChanged(int value)
    {
        SetRegion(regionCodes[value, 1]);
    }
}