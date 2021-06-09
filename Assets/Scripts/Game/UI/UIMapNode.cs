﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PPopGame
{
    public class UIMapNode : MonoBehaviour
    {
        #region Attributes
        [SerializeField]
        private Image _mapImage;
        private Action<int> _pressMapAction;
        private int _index;
        #endregion

        #region Public Methods
        public static void Init(UIMapNode mn) {
            mn.gameObject.SetActive(true);
        }

        public static void Finalize(UIMapNode mn) {
            mn.gameObject.SetActive(false);
        }

        public void SetMapUI(int index, Sprite sprite)
        {
            _mapImage.sprite = sprite;
            _index = index;
        }

        public void MapButtonPress()
        {
            _pressMapAction?.Invoke(_index);
        }

        public void AddMapButtonAction(Action<int> mapButtonAction)
        {
            _pressMapAction = mapButtonAction;
        }
        #endregion
    }
}
