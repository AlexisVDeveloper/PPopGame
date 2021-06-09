using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameUI 
{
    public class UIController : MonoBehaviour
    {
        #region Attributes
        [SerializeField] private UIMapLoaderScreen _mapLoaderScreen;
        [SerializeField] private UIMainScreen _mainScreen;
        [SerializeField] private UIMapScreen _mapScreen;
        #endregion
        
        #region Public Methods
        public void AddGenerateMapAction(Action<float, float> completion) {
            _mainScreen.AddGenerateMapAction(completion);
        }

        public void AddLoadScreenAction(Action completion) {
            _mainScreen.AddLoaderAction(completion);
        }

        public void AddMapButtonAction(Action<int> completion) {
            _mapLoaderScreen.AddMapButtonActions(completion);
        }

        public void AddBackMainAction(Action completion) {
            _mapLoaderScreen.AddBackButtonAction(completion);
        }

        public void ShowMessage(string message, Color color) {
            _mapScreen.SetMessageText(message, color);
        }

        public void AddBackMenuAction(Action completion) {
            _mapScreen.AddBackMainAction(completion);
        }

        public void AddRefreshMapAction(Action completion) {
            _mapScreen.AddRefreshAction(completion);
        }

        public void AddChangeMapAction(Action completion) {
            _mapScreen.AddChangeAction(completion);
        }

        public void AddSaveMapAction(Action completion) {
            _mapScreen.AddSaveAction(completion);
        }

        public void GoToMapScreen() {
            _mainScreen.SetScreenActive(false);
        }

        public void GoToLoadScreen(Sprite[] mapImages) {
            _mainScreen.gameObject.SetActive(false);
            _mapLoaderScreen.SetScreenActive(true, mapImages);
        }

        public void BackFromLoaderScreen() {
            _mapLoaderScreen.SetScreenActive(false);
            _mainScreen.SetScreenActive(true);
        }

        public void BackFromMapScreen() {
            _mainScreen.SetScreenActive(true);
        }

        public void LoadMapAction() {
            _mapLoaderScreen.SetScreenActive(false);
        }
        #endregion
    }
}