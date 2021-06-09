using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;
using PPopGame;

namespace GameMap {
    public class MapNode : MonoBehaviour, IAStarNode
    {
        #region Attributes
        public enum NodeType : int { Water = 0, Grass = 1, Forest = 3, Desert = 5, Mountain = 10 };
        private NodeType _currentType;
        private Renderer _render;
        private int _row, _col;
        private const float _offsetY = 1.4f;
        private IEnumerable<MapNode> _neighbours;
        private const string MATERIAL_PATH = "Materials/m_";
        private const int _newRotationX = -90;
        #endregion

        #region Private Methods
        private void Init() {
            _render = GetComponent<Renderer>();

            transform.eulerAngles = new Vector2(_newRotationX, 0);
            ChangeTypeRandom();
        }

        private void ChangeMaterialTo(NodeType type) {
            _currentType = type;
            if(_render == null) 
                _render = GetComponent<Renderer>();

            _render.material = Resources.Load<Material>(MATERIAL_PATH + type.ToString());
        }
        #endregion

        #region Public Methods
        public static void InitNode(MapNode mn) {
            mn.gameObject.SetActive(true);
            mn.Init();
        }

        public static void DisposeNode(MapNode mn) {
            mn.gameObject.SetActive(false);
        }

        public void SetPosition(int row, int col, Transform startPos) {
            _row = row;
            _col = col;
            name = $"MapNode: [{row},{col}]";

            float offsetX = row % 2 == 1 ? transform.localScale.x / 2 : 0;
            var newPosition = new Vector2(startPos.position.x + offsetX + transform.localScale.x * col,
                                       startPos.position.y + (transform.localScale.y / _offsetY) * row);
            transform.position = newPosition;
        }

        public float DistanceTo(int row, int col) {
            return Mathf.Sqrt(Mathf.Pow(row - _row, 2) + Mathf.Pow(col - _col, 2)); //Diagonal Distance in HexSpace
        } 

        public void SetParent(Transform parent) {
            transform.SetParent(parent);
        }

        public NodeType GetNodeType() {
            return _currentType;
        }

        public void ChangeTypeTo(bool next, NodeType nodeType = NodeType.Water) {
            var type = nodeType;

            if(next) {
                var values = Enum.GetValues(typeof(NodeType));
                var currentIndex = System.Array.IndexOf(values, _currentType);
                currentIndex = (currentIndex + 1) >= values.Length ? 0 : (currentIndex + 1);
                type = (NodeType)values.GetValue(currentIndex);
            }

            ChangeMaterialTo(type);
        }

        public void ChangeTypeRandom() {
            var values = Enum.GetValues(typeof(NodeType));
            int seed = UnityEngine.Random.Range(0, int.MaxValue);
            var random = new System.Random(seed);
            var randomType = (NodeType)values.GetValue(random.Next(values.Length));
            ChangeMaterialTo(randomType);
        }

        public void SetNeighbours(MapNode[,] map) {
            List<MapNode> result = new List<MapNode>();
            int rowMinimum = _row - 1 < 0 ? _row : _row - 1;
            int rowMaximum = _row + 1 >= map.GetLength(0) ? _row : _row + 1;
            int columnMinimum = _col - 1 < 0 ? _col : _col - 1;
            int columnMaximum = _col + 1 >= map.GetLength(1) ? _col : _col + 1;

            for (int i = rowMinimum; i <= rowMaximum; i++)
                for (int j = columnMinimum; j <= columnMaximum; j++)
                    if (i != _row || j != _col) 
                        if(_row % 2 <= 0 && (i == _row + 1 && j == _col + 1 || i == _row - 1 && j == _col + 1) || 
                        (_row % 2 >= 1 && (i == _row - 1 && j == _col - 1 || i == _row + 1 && j == _col - 1)))
                            continue;
                        else
                            result.Add(map[i,j]);

            _neighbours = result.Where(node => {
                return node != null && node.GetNodeType() != NodeType.Water;
            });
        }
        
        public IEnumerable<IAStarNode>	Neighbours { get {
            return _neighbours;
        }}

        public float CostTo(IAStarNode neighbour) {
            return (int)_currentType;
        }
        
		public float EstimatedCostTo(IAStarNode target) {
            var targetNode = (MapNode)target;
            return targetNode.DistanceTo(_row, _col);
        }

        public void ChangeColorTo(Color newColor) {
            _render.material.color = newColor;
        }
        #endregion
    }
}
