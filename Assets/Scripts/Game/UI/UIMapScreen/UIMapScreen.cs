using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameUI {
    public class UIMapScreen : MonoBehaviour
    {
        #region Attributes
        [SerializeField] private TextMeshProUGUI _messageTxt;
        [SerializeField] private Button _refreshButton, _changeButton, _saveButton, _backButton;
        
        private const float _timeToDesactive = 4f;
        private IEnumerator _desactiveMessageCoroutine;
        private Action _refreshAction, _changeAction, _saveAction, _backMainAction;
        #endregion

        #region Public Methods
        public void SetMessageText(string text, Color color) {
            _messageTxt.text = text;
            _messageTxt.gameObject.SetActive(true);
            _messageTxt.color = color;

            if(_desactiveMessageCoroutine != null) 
                StopCoroutine(_desactiveMessageCoroutine);

            _desactiveMessageCoroutine = DesactiveMessage();
            StartCoroutine(_desactiveMessageCoroutine);
        }

        public void AddRefreshAction(Action completion) {
            _refreshAction = completion;
        }

        public void AddChangeAction(Action completion) {
            _changeAction = completion;
        }

        public void AddSaveAction(Action completion) {
            _saveAction = completion;
        }

        public void AddBackMainAction(Action completion) {
            _backMainAction = completion;
        }
        #endregion

        #region Private Region
        private void Start() {
            _refreshButton.onClick.AddListener(() => { _refreshAction?.Invoke(); });
            _changeButton.onClick.AddListener(() => { _changeAction?.Invoke(); });
            _saveButton.onClick.AddListener(() => { _saveAction?.Invoke(); });
            _backButton.onClick.AddListener(() => { _backMainAction?.Invoke(); });
        }

        private IEnumerator DesactiveMessage() {
            yield return new WaitForSeconds(_timeToDesactive);
            _messageTxt.gameObject.SetActive(false);
            _desactiveMessageCoroutine = null;
        }
        #endregion
    }
}
