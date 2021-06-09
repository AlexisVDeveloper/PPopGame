﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPopGame {
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Material _imageEffectMaterial;
        [SerializeField] private float _time;
        private Camera _currentCamera;
        private int _width = 160, _height = 160;
        private bool _takeScreenshotOnNextFrame;
        private Action<Texture2D> _completion;

        #region Public Methods
        public void StartTransition(int startValue, bool isFadeout) {
            _imageEffectMaterial.SetFloat("_CutOff", startValue);
            if(isFadeout) 
                StartCoroutine(FadeoutTransition());
            else
                StartCoroutine(FadeinTransition());
        }

        public void TakeScreenshot(Action<Texture2D> completion) {
            _completion = completion;
            _currentCamera.targetTexture = RenderTexture.GetTemporary(_width, _height, 20);
            _takeScreenshotOnNextFrame = true;
        }
        #endregion
 
        #region Private Methods
        private void Start() {
            _currentCamera = GetComponent<Camera>();
        }
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _imageEffectMaterial);
        }

        private IEnumerator FadeoutTransition() {
            var i = _imageEffectMaterial.GetFloat("_CutOff") * _time; 
            while(_imageEffectMaterial.GetFloat("_CutOff") > 0) {
                _imageEffectMaterial.SetFloat("_CutOff", i / _time);
                yield return new WaitForEndOfFrame();
                i--;
            }
        }

        private IEnumerator FadeinTransition() {
            var i = 0; 
            while(_imageEffectMaterial.GetFloat("_CutOff") < 1) {
                _imageEffectMaterial.SetFloat("_CutOff", i * _time);
                yield return new WaitForEndOfFrame();
                i++;
            }
        }

        private void OnPostRender() {
            if(_takeScreenshotOnNextFrame) {
                _takeScreenshotOnNextFrame = false;

                RenderTexture renderTexture = _currentCamera.targetTexture;
                Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
                Rect rect = new Rect(0,0, renderTexture.width, renderTexture.height);
                renderResult.ReadPixels(rect, 0, 0);

                _completion?.Invoke(renderResult);
                RenderTexture.ReleaseTemporary(renderTexture);
                _currentCamera.targetTexture = null;
            }
        }
        #endregion
    }
}