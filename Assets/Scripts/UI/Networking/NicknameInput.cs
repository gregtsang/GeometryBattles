using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GeometryBattles.Networking;

namespace GeometryBattles.MenuUI
{
    public class NicknameInput : MonoBehaviour
    {
        void Start()
        {
            TMP_InputField inputField;   
            inputField = GetComponent<TMP_InputField>();
            inputField.text = MasterManager.GameSettings.nickname;
            inputField.onValueChanged.AddListener(delegate {
                InputFieldValueChanged(inputField.text);
            });
        }

        void InputFieldValueChanged(string value)
        {
            ServerConnectionSettings.UpdateNickName(value);
        }
    }
}