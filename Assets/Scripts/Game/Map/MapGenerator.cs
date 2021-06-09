using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private string _currentIndexCode;
        private int _row, _col;
        private Action _completion;
        private const string MAPNODE_PATH = "Prefabs/MapNode";
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
            _currentIndexCode = null;
            StartCoroutine(DesactiveMap());
        }

        public void ChangeMap()
        {
            _currentIndexCode = null;
            StartCoroutine(ChangeAllMap());
        }

        public void SaveMapImage(Texture2D image, string codeIndex) {
            MapSaveLoad.SaveImageMap(image, codeIndex);
        }

        public void SaveMap(Action<string> completion)
        {
            completion += (codeIndex) => { _currentIndexCode = codeIndex; };
            StartCoroutine(MapSaveLoad.SaveMapData(completion, _currentMap, _currentIndexCode));
        }

        public void LoadMapsImages(Action<Sprite[]> completion)
        {
            StartCoroutine(MapSaveLoad.GetAllMapsImages(completion));
        }

        public void LoadMapWithIndex(int index, Action completion) {
            var mapNodeSave = MapSaveLoad.GetMapWithIndex(index);
            _currentIndexCode = mapNodeSave.indexCode;
            _row = mapNodeSave.mapNodes.GetLength(0);
            _col = mapNodeSave.mapNodes.GetLength(1);
            _completion = completion;

            StartCoroutine(GenerateNode(mapNodeSave.mapNodes)); 
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
        #endregion
    }
}
