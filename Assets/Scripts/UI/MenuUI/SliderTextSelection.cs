using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryBattles.MenuUI
{
    public class SliderTextSelection : MonoBehaviour
    {
        //Cached References
        Slider slider;
        TMP_InputField inputField;
        
        public void SliderUpdate()
        {
            if (inputField.text != slider.value.ToString())
            {
                inputField.text = slider.value.ToString();
            }
        }
        
        public void InputUpdate()
        {
            int inputValue;
            if (int.TryParse(inputField.text, out inputValue))
            {
                if (slider.value != inputValue)
                {
                    slider.value = inputValue;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            slider = GetComponentInChildren<Slider>();
            inputField = GetComponentInChildren<TMP_InputField>();
            SliderUpdate();
        }
    }
}