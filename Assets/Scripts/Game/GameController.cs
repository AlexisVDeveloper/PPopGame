using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameMap;
using PathFinding;
using GameUI;

namespace PPopGame {
    public class GameController : MonoBehaviour
    {
        #region Attributes
        [Header("Map Parameters")]
        [SerializeField] private MapGenerator _mapGenerator;
        [Header("UI Controller")]
        [SerializeField] private UIController _UIController;
        [Header("Camera Controller")]
        [SerializeField] private CameraController _cameraController;
        private List<MapNode> _allPaths;
        private MapNode _start, _end;
        private const string _errorPathMessage = "We can't find a path to that position.";
        private const string _saveMapMessage = "The Map was saved correctly.";
        private const string _saveMapErrorMessage = "The Map already exist.";
        #endregion

        #region Private Methods
        private void Start() {
            _allPaths = new List<MapNode>();
            ConfigureUIController();
        }

        private void ConfigureUIController() {
            _UIController.AddRefreshMapAction(RefreshMap);

            _UIController.AddChangeMapAction(() => {
                RefreshMap();
                _mapGenerator.ChangeMap();
            });

            _UIController.AddSaveMapAction(() => {
                RefreshMap();
                 _mapGenerator.SaveMap((index) => {
                    _cameraController.TakeScreenshot((text2D) => {
                        _mapGenerator.SaveMapImage(text2D, index);
                        _UIController.ShowMessage(_saveMapMessage, Color.green);
                    });
                });
            });

            _UIController.AddBackMenuAction(() => { 
                _mapGenerator.Desactive();
                _UIController.BackFromMapScreen();
            });

            _UIController.AddGenerateMapAction((row, col) => {
                _mapGenerator.Generate((int)row, (int)col, () => { 
                    _UIController.GoToMapScreen();
                    _cameraController.StartTransition();
                });
            });
            
            _UIController.AddLoadScreenAction(() => {
                _mapGenerator.LoadMapsImages( (sprites) => { _UIController.GoToLoadScreen(sprites); });
            });

            _UIController.AddMapButtonAction((index) => {
                _mapGenerator.LoadMapWithIndex(index, () => {
                    _UIController.LoadMapAction();
                    _cameraController.StartTransition();
                });
            });

            _UIController.AddBackMainAction(() => { _UIController.BackFromLoaderScreen(); });
        }

        private void Update() {
            if(Input.GetMouseButtonDown(0))
                SetStartAndEnd(GetNodeInMap());
            else if(Input.GetMouseButtonDown(1)) 
                RestartStart(GetNodeInMap());
            else if(Input.GetMouseButtonDown(2))
                ChangeNodeType();
        }

        private MapNode GetNodeInMap() {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
            if(Physics.Raycast(ray, out hit)) {
                return hit.collider.gameObject.GetComponent<MapNode>();
            }

            return null;
        }

        private void ChangeNodeType() {
            var node = GetNodeInMap();
            if(node != null)
                node.ChangeTypeTo(next: true);
        }

        private void RestartStart(MapNode node) {
            if(node == null || node.GetNodeType() == MapNode.NodeType.Water)
                return;
                
            if(_start != null && node != _start && _start != _end) {
                _start.ChangeColorTo(Color.white);
                _allPaths.Remove(_start);
            }
            
            _start = node;
            _allPaths.Add(_start);
            _start.ChangeColorTo(Color.green);
        }

        private void SetStartAndEnd(MapNode node) {
            if(node == null || node.GetNodeType() == MapNode.NodeType.Water) 
                return;

            if(_start == null) {
                _start = node;
                _allPaths.Add(_start);
                _start.ChangeColorTo(Color.green);
            } else if(node != _start) {
                _end = node;
                _allPaths.Add(_end);
                _end.ChangeColorTo(Color.green);

                StartSearch();
            }
        }

        private void StartSearch() {
            try {
                var currentPath = SearchPath().Select( node => {  
                    var newNode = (MapNode)node;

                    if(newNode != _start && newNode != _end)
                        newNode.ChangeColorTo(Color.red);
                        
                    return newNode;
                });

                foreach(var node in currentPath) {
                    if(!_allPaths.Contains(node))
                        _allPaths.Add(node);
                }
            } catch(Exception e) {
                _UIController.ShowMessage(_errorPathMessage, Color.red);
            } finally {
                _start = _end;
            }
        }

        private void RefreshMap() {
            if(_allPaths.Count < 1)
                return;

            foreach(var node in _allPaths) {
                node.ChangeColorTo(Color.white);
            }

            _allPaths = new List<MapNode>();
            _start = null;
            _end = null;
        }

        private IList<IAStarNode> SearchPath() {
            return AStar.GetPath(_start, _end);
        }
        #endregion
    }
}