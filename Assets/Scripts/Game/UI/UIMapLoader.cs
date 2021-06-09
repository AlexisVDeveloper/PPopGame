using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ObjectsPool;

namespace PPopGame
{
    public class UIMapLoader : MonoBehaviour
    {
        #region Attributes
        [SerializeField]
        private Transform _gridParent;
        private const string PATH_NODE = "Prefabs/MapNodeUI";
        private Action<int> _mapButtonAction;
        private Pool<UIMapNode> _poolNodes;
        private List<UIMapNode> _nodes;
        #endregion

        #region  Public Methods
        public void StartLoader(Sprite[] mapsImage) {
            if(_poolNodes == null) 
                _poolNodes = new Pool<UIMapNode>(FactoryNode, UIMapNode.Init, UIMapNode.Finalize);

            _nodes = new List<UIMapNode>();

            StartCoroutine(InstanceNodes(mapsImage));
        }

        public void DesactiveLoaderMaps() {
            foreach(var node in _nodes) {
                _poolNodes.DesactivePoolObject(node);
            }

            _nodes = null;
        }

        public void AddMapButtonAction(Action<int> mapButtonAction) {
            DesactiveLoaderMaps();
            _mapButtonAction = mapButtonAction;
        } 
        #endregion

        #region Private Methods
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

        private UIMapNode FactoryNode() {
            var prefab = Resources.Load<UIMapNode>(PATH_NODE);
            return Instantiate(prefab, _gridParent.position, Quaternion.identity);
        }
        #endregion
    }
}
