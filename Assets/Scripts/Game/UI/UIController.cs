using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PPopGame {
    public class UIController : MonoBehaviour
    {
        #region Attributes
        [SerializeField] private Image _mainScreen, _loadScreen;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private TextMeshProUGUI _messageTxt, _generateTxt;
        [SerializeField] private UIMapLoader _mapLoaderController;
        private const float _timeToDesactive = 4f;
        private IEnumerator _desactiveMessageCoroutine;
        private Action _restartMapAction, _changeMapAction, _saveMapAction, _loadMapAction, _generateMapAction;
        #endregion

        #region Public Methods
        public void GoToMapScreen() {
            ActiveMainScreen(false);
            ActiveLoadScreen(false);
            _cameraController.StartTransition();
        }

        public void GoToLoadScreen(Sprite[] mapSprites) {
            ActiveMainScreen(false);
            ActiveLoadScreen(true);
            _cameraController.StartTransition();
            _mapLoaderController.StartLoader(mapSprites);
        }

        public void GoToMainScreen() {
            ActiveLoadScreen(false);
            ActiveMainScreen(true);
            _cameraController.StartTransition();
        }

        public void AddChangeMapAction(Action changeMap) {
            _changeMapAction = changeMap;
        }

        public void AddRestartMapAction(Action restartMap) {
            _restartMapAction = restartMap;
        }

        public void AddSaveMapAction(Action saveMap) {
            _saveMapAction = saveMap;
        }

        public void AddLoadMapAction(Action loadMap) {
            _loadMapAction = loadMap;
        }

        public void AddGenerateMapAction(Action generateMap) {
            _generateMapAction = generateMap;
        }

        public void AddMapButtonAction(Action<int> mapButtonAction) {
            _mapLoaderController.AddMapButtonAction(mapButtonAction);
        }

        public void RestartMapButtonPress() {
            _restartMapAction?.Invoke();
        }

        public void ChangeMapButtonPress() {
            _changeMapAction?.Invoke();
        }

        public void SaveMapButtonPress() {
            _saveMapAction?.Invoke();
        }

        public void LoadMapButtonPress() {
            _loadMapAction?.Invoke();
        }

        public void GenerateButtonPress() {
            _generateTxt.gameObject.SetActive(true);
            _generateMapAction?.Invoke();
        }

        public void BackButtonPress() {
            _mapLoaderController.DesactiveMap();
            GoToMainScreen();
        }

        public void SetMessageText(string text, Color color) {
            _messageTxt.text = text;
            _messageTxt.gameObject.SetActive(true);
            _messageTxt.color = color;

            if(_desactiveMessageCoroutine != null) 
                StopCoroutine(_desactiveMessageCoroutine);

            _desactiveMessageCoroutine = DesactiveMessage();
            StartCoroutine(_desactiveMessageCoroutine);
        }

        public void TakeScreenshot(Action<Texture2D> completion) {
            _cameraController.TakeScreenshot(completion);
        }
        #endregion

        #region Private Methods
        private IEnumerator DesactiveMessage() {
            yield return new WaitForSeconds(_timeToDesactive);
            _messageTxt.gameObject.SetActive(false);
            _desactiveMessageCoroutine = null;
        }

        private void ActiveMainScreen(bool value) {
            _mainScreen.gameObject.SetActive(value);
        }

        private void ActiveLoadScreen(bool value) {
            _loadScreen.gameObject.SetActive(value);
        }
        #endregion
    }
}