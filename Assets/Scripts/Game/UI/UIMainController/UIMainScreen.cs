using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameUI
{
    public class UIMainScreen : MonoBehaviour
    {
        #region Attributes
        [SerializeField] private Slider _rowSlider, _colSlider;
        [SerializeField] private TextMeshProUGUI _rowText, _colText;
        [SerializeField] private Button _mapButton, _loaderButton;
        [SerializeField] private TextMeshProUGUI _helperTxt;
        private Action<float, float> _mapButtonAction;
        private Action _loaderButtonAction;
        #endregion

        #region Private Methods
        private void Start()
        {
            _mapButton.onClick.AddListener(() => { 
                _mapButtonAction?.Invoke(_rowSlider.value, _colSlider.value);
            });
            
            _loaderButton.onClick.AddListener(() => { 
                _loaderButtonAction?.Invoke();
            });

            _rowSlider.onValueChanged.AddListener((row) => { 
                _rowText.text = $"Row: {row}";
            });

            _colSlider.onValueChanged.AddListener((col) => {
                _colText.text = $"Col: {col}";
            });
        }
        #endregion

        #region Public Methods
        public void AddGenerateMapAction(Action<float, float> completion) {
            _mapButtonAction = completion;
        }

        public void AddLoaderAction(Action loaderAction) {
            _loaderButtonAction = loaderAction;
        }

        public void SetScreenActive(bool value) {
            gameObject.SetActive(value);
        }

        public void HelperTextSetActive(bool value)
        {
            _helperTxt.gameObject.SetActive(value);
        }
        #endregion
    }
}
