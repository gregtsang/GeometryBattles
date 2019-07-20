﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GeometryBattles.MenuUI
{
    public class ButtonRequireText : MonoBehaviour
    {
        [SerializeField] TMP_InputField textInputField = null;
        [SerializeField] int minChars = 3; 
        
        Button button;

        public void InputTextUpdate()
        {
            if (textInputField.text.Length >= minChars)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();
            InputTextUpdate();
        }
    }
}