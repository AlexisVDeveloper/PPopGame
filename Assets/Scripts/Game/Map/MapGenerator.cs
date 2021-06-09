using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GameSaveLoad;
using ObjectsPool;

namespace GameMap
{
    public class MapGenerator : MonoBehaviour
    {
        #region Attributes
        [SerializeField] private Transform _startPosition;
        private GameObject[] _parents;
        private MapNode[,] _currentMap;
        private Pool<MapNode> _mapNodePool;
        private Pool<GameObject> _nodeParentPool;
        private int _row, _col;
        private const int _maxCharForCode = 15;
        private string _currentIndexCode;
        private Action _completion;
        private const string MAPNODE_PATH = "Prefabs/MapNode";
        private const string SAVE_NAME = "save_maps";
        private const string IMAGE_NAME = "map_";
        #endregion

        #region Public Methods
        public void Generate(int row, int col, Action completion)
        {
            _completion = completion;

            _row = row;
            _col = col;

            StartCoroutine(GenerateNode());
        }

        public void Desactive()
        {
            StartCoroutine(DesactiveMap());
        }

        public void ChangeMap()
        {
            _currentIndexCode = null;
            StartCoroutine(ChangeAllMap());
        }

        public void SaveMap(Action<int> completion)
        {
            StartCoroutine(SaveMapData(completion));
        }

        public void SaveImageMap(Texture2D image, int index)
        {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);

            if (data != null)
                IOSaveLoad<Texture2D>.SaveImageData(image, IMAGE_NAME + index);
        }

        public void LoadMapsImages(Action<Sprite[]> completion)
        {
            StartCoroutine(GetAllMapsImages(completion));
        }

        public void LoadMapWithIndex(Action completion, int index)
        {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);
            var mapNodes = data.maps[index].mapNodes;
            _row = mapNodes.GetLength(0);
            _col = mapNodes.GetLength(1);

            _completion = completion;
            _currentIndexCode = data.maps[index].indexCode;
            StartCoroutine(GenerateNode(mapNodes));
        }
        #endregion

        #region Private Methods
        private void Start()
        {
            _mapNodePool = new Pool<MapNode>(InstantiateNode, MapNode.InitNode, MapNode.DisposeNode);
            _nodeParentPool = new Pool<GameObject>(InstantiateParent, InitParent, FinalizeParent);
        }

        private IEnumerator GenerateNode(MapNodeData[,] savedNode = null)
        {
            _currentMap = new MapNode[_row, _col];
            _parents = new GameObject[_row];

            for (var i = 0; i < _row; i++)
            {
                _parents[i] = _nodeParentPool.GetObject().GetObj;
                SetNodeParent(i, _parents[i]);

                for (var j = 0; j < _col; j++)
                {
                    var newNode = _mapNodePool.GetObject().GetObj;
                    newNode.SetPosition(i, j, _startPosition);
                    newNode.SetParent(_parents[i].transform);
                    _currentMap[i, j] = newNode;

                    if (savedNode != null) _currentMap[i, j].ChangeTypeTo(next: false, savedNode[i, j].currentType);
                    yield return new WaitForEndOfFrame();
                }
            }

            StartCoroutine(LookForNeighbours());
        }

        private IEnumerator LookForNeighbours()
        {
            for (var i = 0; i < _row; i++)
            {
                for (var j = 0; j < _col; j++)
                {
                    _currentMap[i, j].SetNeighbours(_currentMap);
                    yield return new WaitForEndOfFrame();
                }
            }
            _completion?.Invoke();
        }

        private IEnumerator ChangeAllMap()
        {
            for (var i = 0; i < _currentMap.GetLength(0); i++)
            {
                for (var j = 0; j < _currentMap.GetLength(1); j++)
                {
                    _currentMap[i, j].ChangeTypeRandom();
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        private IEnumerator DesactiveMap()
        {
            for (var i = 0; i < _currentMap.GetLength(0); i++)
            {
                _nodeParentPool.DesactivePoolObject(_parents[i]);
                for (var j = 0; j < _currentMap.GetLength(1); j++)
                {
                    _mapNodePool.DesactivePoolObject(_currentMap[i, j]);
                    yield return new WaitForEndOfFrame();
                }
            }

            _currentIndexCode = null;
        }

        private IEnumerator SaveMapData(Action<int> completion)
        {
            var oldData = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);

            var row = _currentMap.GetLength(0);
            var col = _currentMap.GetLength(1);

            SaveData data = oldData == null ? new SaveData() : oldData;
            data.maps = oldData == null ? new List<MapData>() : oldData.maps;

            var _map = new MapData();
            _map.mapNodes = new MapNodeData[row, col];

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < col; j++)
                {
                    var newNode = new MapNodeData();
                    newNode.currentType = _currentMap[i, j].GetNodeType();
                    _map.mapNodes[i, j] = newNode;
                    yield return new WaitForEndOfFrame();
                }
            }

            var index = 0;

            if (_currentIndexCode != null)
            {
                for (var i = 0; i < data.maps.Count; i++)
                {
                    if (data.maps[i].indexCode == _currentIndexCode)
                    {
                        _map.indexCode = _currentIndexCode;
                        data.maps[i] = _map;
                        index = i;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                _currentIndexCode = GenerateIndexCode();
                _map.indexCode = _currentIndexCode;
                data.maps.Add(_map);
                index = (data.maps.Count - 1);
            }

            IOSaveLoad<SaveData>.SaveData(data, SAVE_NAME);
            completion?.Invoke(index);
        }

        private IEnumerator GetAllMapsImages(Action<Sprite[]> completion)
        {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);

            if (data != null)
            {
                var result = new Sprite[data.maps.Count];
                var imagesCount = data.maps.Count;

                for (var i = 0; i < imagesCount; i++)
                {
                    var text2D = IOSaveLoad<Texture2D>.LoadImage(IMAGE_NAME + i);
                    if (text2D != null)
                    {
                        result[i] = Sprite.Create(text2D, new Rect(0.0f, 0.0f, text2D.width, text2D.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                    yield return new WaitForEndOfFrame();
                }
                completion?.Invoke(result);
            }
            else
                completion?.Invoke(new Sprite[0]);
        }

        private MapNode InstantiateNode()
        {
            var prefab = Resources.Load<MapNode>(MAPNODE_PATH);
            return Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        private GameObject InstantiateParent()
        {
            var newParent = new GameObject();
            return newParent;
        }

        private void SetNodeParent(int row, GameObject parent)
        {
            var position = new Vector2(_startPosition.position.x, _startPosition.position.y * row);
            parent.transform.SetParent(transform);
            parent.transform.position = position;
            parent.name = $"Parent Row: {row}";
        }

        private static void InitParent(GameObject parent)
        {
            parent.gameObject.SetActive(true);
        }

        private static void FinalizeParent(GameObject parent)
        {
            parent.gameObject.SetActive(false);
        }

        private string GenerateIndexCode()
        {
            var characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = "MAP";
            for (var i = 0; i < _maxCharForCode; i++)
            {
                var charIndex = UnityEngine.Random.Range(0, characters.Length);
                result += characters[charIndex];
            }
            return result;
        }
        #endregion
    }
}
