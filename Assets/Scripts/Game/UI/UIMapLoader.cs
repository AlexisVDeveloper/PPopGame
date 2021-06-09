using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPopGame
{
    public class UIMapLoader : MonoBehaviour
    {
        #region Attributes
        [SerializeField]
        private Transform _gridParent;
        private List<UIMapNode> _nodes;
        private const string PATH_NODE = "Prefabs/MapNodeUI";
        private Action<int> _mapButtonAction;
        #endregion

        #region  Public Methods
        public void StartLoader(Sprite[] mapsImage) {
            if(_nodes == null) 
                _nodes = new List<UIMapNode>();

            if(_nodes.Count < mapsImage.Length) 
                StartCoroutine(InstanceNodes(mapsImage));
            else 
                StartCoroutine(SetActiveNodes(true));
        }

        public void DesactiveMap() {
            StartCoroutine(SetActiveNodes(false));
        }

        public void AddMapButtonAction(Action<int> mapButtonAction) {
            _mapButtonAction = mapButtonAction;
        } 
        #endregion

        #region Private Methods
        private IEnumerator InstanceNodes(Sprite[] mapImages) {
            var prefab = Resources.Load<UIMapNode>(PATH_NODE);
            for(var i = _nodes.Count; i < mapImages.Length; i++) {
                var newNode = Instantiate(prefab, _gridParent.position, Quaternion.identity);
                newNode.SetMapUI(i, mapImages[i]);
                newNode.transform.SetParent(_gridParent);
                newNode.AddMapButtonAction(_mapButtonAction);
                _nodes.Add(newNode);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator SetActiveNodes(bool value) {
            foreach(var node in _nodes) {
                node.gameObject.SetActive(value);
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}
