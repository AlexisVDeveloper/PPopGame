using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GameSaveLoad;

namespace GameMap
{
    public class MapGenerator : MonoBehaviour
    {
        #region Attributes
        [SerializeField] private Transform _startPosition;
        private MapNode[,] _currentMap;
        private string _currentIndexCode;
        private const string MAPNODE_PATH = "Prefabs/MapNode";
        private const int _maxCharForCode = 15;
        private const float _offsetY = 1.4f;
        private Action _completion;
        private const string SAVE_NAME = "save_maps";
        private const string IMAGE_NAME = "map_";
        #endregion

        #region Public Methods
        public void Generate(int row, int col, Action completion) {
            _completion = completion;
            StartCoroutine(GenerateNode(row, col));
        }

        public void ChangeMap() {
            _currentIndexCode = null;
            StartCoroutine(ChangeAllMap());
        }

        public void SaveMap(Action<int> completion) {
            StartCoroutine(SaveMapData(completion));
        }

        public void SaveImageMap(Texture2D image, int index) {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);
            
            if(data != null)
                IOSaveLoad<Texture2D>.SaveImageData(image, IMAGE_NAME + index);
        }

        public void LoadMapsImages(Action<Sprite[]> completion) {
            StartCoroutine(GetAllMapsImages(completion));
        }

        public void LoadMapWithIndex(Action completion, int index) {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);
            var mapNodes = data.maps[index].mapNodes;
            var row = mapNodes.GetLength(0);
            var col = mapNodes.GetLength(1);

            _completion = completion;
            _currentIndexCode = data.maps[index].indexCode;
            StartCoroutine(GenerateNode(row, col, mapNodes));
        }
        #endregion

        #region Private Methods
        private IEnumerator GenerateNode(int row, int col, MapNodeData[,] savedNode = null) {
            _currentMap = new MapNode[row, col];
            var prefab = Resources.Load<MapNode>(MAPNODE_PATH);

            for(var i = 0; i < row; i++) {
                var currentParent = InstantiateParent(i);
                for(var j = 0; j < col; j++) {
                    if(savedNode != null)
                        _currentMap[i, j] = InstantiateNode(i, j, prefab, currentParent.transform, savedNode[i,j].currentType);
                    else
                        _currentMap[i, j] = InstantiateNode(i, j, prefab, currentParent.transform);
                    yield return new WaitForEndOfFrame();
                }
            }
            
            StartCoroutine(LookForNeighbours(row, col));
        }

        private IEnumerator LookForNeighbours(int row, int col) {
             for(var i = 0; i < row; i++) {
                for(var j = 0; j < col; j++) {
                    _currentMap[i, j].SetNeighbours(_currentMap);
                    yield return new WaitForEndOfFrame();
                }
            }
            _completion?.Invoke();
        }

        private IEnumerator ChangeAllMap() {
            for(var i = 0; i < _currentMap.GetLength(0); i++) {
                for(var j = 0; j < _currentMap.GetLength(1); j++) {
                    _currentMap[i, j].ChangeTypeRandom();
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        private IEnumerator SaveMapData(Action<int> completion) {
            var oldData = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);

            var row = _currentMap.GetLength(0);
            var col = _currentMap.GetLength(1);

            SaveData data = oldData == null ? new SaveData() : oldData;
            data.maps = oldData == null ? new List<MapData>() : oldData.maps;
            
            var _map = new MapData();
            _map.mapNodes = new MapNodeData[row, col];

            for(var i = 0; i < row; i++) {
                for(var j = 0; j < col; j++) {
                    var newNode = new MapNodeData();
                    newNode.currentType = _currentMap[i,j].GetNodeType(); 
                    _map.mapNodes[i,j] = newNode;
                    yield return new WaitForEndOfFrame();
                }
            }

            var index = 0;

            if(_currentIndexCode != null) {
                for(var i = 0; i < data.maps.Count; i++) {
                    if(data.maps[i].indexCode == _currentIndexCode) {
                        _map.indexCode = _currentIndexCode;
                        data.maps[i] = _map;
                        index = i;
                    }

                    yield return new WaitForEndOfFrame();
                } 
            } else {
                _currentIndexCode = GenerateIndexCode();
                _map.indexCode = _currentIndexCode;
                data.maps.Add(_map);
                index = (data.maps.Count - 1);
            }

            IOSaveLoad<SaveData>.SaveData(data, SAVE_NAME);
            completion?.Invoke(index);
        }

        private IEnumerator GetAllMapsImages(Action<Sprite[]> completion) {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);
            
            if(data != null) {
                var result = new Sprite[data.maps.Count];
                var imagesCount = data.maps.Count;
                
                for(var i = 0; i < imagesCount; i++) {
                    var text2D = IOSaveLoad<Texture2D>.LoadImage(IMAGE_NAME + i);
                    if(text2D != null) { 
                        result[i] = Sprite.Create(text2D, new Rect(0.0f, 0.0f, text2D.width, text2D.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                    yield return new WaitForEndOfFrame();
                }
                completion?.Invoke(result);
            } else 
                completion?.Invoke(new Sprite[0]);
        }

        private MapNode InstantiateNode(int row, int col, MapNode prefab, Transform parent) {
            float offsetX = row % 2 == 1 ? prefab.transform.localScale.x / 2 : 0;
            var position = new Vector2(_startPosition.position.x + offsetX + prefab.transform.localScale.x * col,
                                       _startPosition.position.y + (prefab.transform.localScale.y / _offsetY) * row);
            var newNode = Instantiate(prefab, position, Quaternion.identity);
            newNode.SetPosition(row,col);
            newNode.SetParent(parent);
            return newNode;
        }

        private MapNode InstantiateNode(int row, int col, MapNode prefab, Transform parent, MapNode.NodeType nodeType) {
            var newNode = InstantiateNode(row, col, prefab, parent);
            newNode.ChangeTypeTo(next: false, nodeType);
            return newNode;
        }

        private GameObject InstantiateParent(int row) {
            var position = new Vector2(_startPosition.position.x, _startPosition.position.y * row);
            var newParent = new GameObject(); 
            newParent.transform.SetParent(transform);
            newParent.transform.position = position;
            newParent.name = $"Parent Row: {row}";
            return newParent;
        }

        private string GenerateIndexCode() {
            var characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = "MAP";
            for(var i = 0; i < _maxCharForCode; i++) {
                var charIndex = UnityEngine.Random.Range(0, characters.Length);
                result += characters[charIndex];
            }
            return result;
        }
        #endregion
    }
}
