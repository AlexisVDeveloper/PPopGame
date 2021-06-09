using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ObjectsPool;

namespace GameUI
{
    public class UIMapLoaderScreen : MonoBehaviour
    {
        #region Attributes
        [SerializeField]
        private Transform _gridParent;
        [SerializeField]
        private Button _backButton;
        private const string PATH_NODE = "Prefabs/MapNodeUI";
        private Action<int> _mapButtonAction;
        private Action _backButtonAction;
        private Pool<UIMapLoaderNode> _poolNodes;
        private List<UIMapLoaderNode> _nodes;
        #endregion

        #region  Public Methods
        public void SetScreenActive(bool value, Sprite[] mapImages = default) {
            gameObject.SetActive(value);
            if(value)
                StartLoader(mapImages);
            else 
                DesactiveLoaderMaps();
        }

        public void AddMapButtonActions(Action<int> mapButtonsAction) {
            _mapButtonAction = mapButtonsAction;
        }

        public void AddBackButtonAction(Action backButtonAction) {
            _backButtonAction = backButtonAction;
        }

        #endregion

        #region Private Methods
        private void Start() {
            _backButton.onClick.AddListener(() => { _backButtonAction?.Invoke(); } );
        }

        private void StartLoader(Sprite[] mapsImage) {
            if(_poolNodes == null) 
                _poolNodes = new Pool<UIMapLoaderNode>(FactoryNode, UIMapLoaderNode.Init, UIMapLoaderNode.Finalize);

            _nodes = new List<UIMapLoaderNode>();

            StartCoroutine(InstanceNodes(mapsImage));
        }

        private void DesactiveLoaderMaps() {
            foreach(var node in _nodes) {
                _poolNodes.DesactivePoolObject(node);
            }
        }

        private IEnumerator InstanceNodes(Sprite[] mapImages) {
            for(var i = 0; i < mapImages.Length; i++) {
                var newNode = _poolNodes.GetObject().GetObj;
                newNode.SetMapUI(i, mapImages[i]);
                newNode.transform.SetParent(_gridParent);
                newNode.AddMapButtonAction(_mapButtonAction);
                _nodes.Add(newNode);
                yield return new WaitForEndOfFrame();
            }
        }

        private UIMapLoaderNode FactoryNode() {
            var prefab = Resources.Load<UIMapLoaderNode>(PATH_NODE);
            return Instantiate(prefab, _gridParent.position, Quaternion.identity);
        }
        #endregion
    }
}
