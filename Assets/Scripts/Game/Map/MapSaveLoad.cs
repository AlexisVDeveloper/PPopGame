using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSaveLoad;

namespace GameMap {
    public class MapSaveLoad {
        #region Attributes
        private const string SAVE_NAME = "save_maps";
        private const string IMAGE_NAME = "map_";
        private const int _maxCharForCode = 15;
        #endregion

        #region Public Methods
        public static MapData GetMapWithIndex(int index)
        {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);
            var mapNodes = data.maps[index];
            return mapNodes;
        }

        public static void SaveImageMap(Texture2D image, string codeIndex)
        {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);

            if (data != null)
                IOSaveLoad<Texture2D>.SaveImageData(image, IMAGE_NAME + codeIndex);
        }

        public static IEnumerator SaveMapData(Action<string> completion, MapNode[,] currentMap, string currentIndexCode)
        {
            var oldData = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);

            var row = currentMap.GetLength(0);
            var col = currentMap.GetLength(1);

            SaveData data = oldData == null ? new SaveData() : oldData;
            data.maps = oldData == null ? new List<MapData>() : oldData.maps;

            var _map = new MapData();
            _map.mapNodes = new MapNodeData[row, col];

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < col; j++)
                {
                    var newNode = new MapNodeData();
                    newNode.currentType = currentMap[i, j].GetNodeType();
                    _map.mapNodes[i, j] = newNode;
                }
            }

            string indexCode = currentIndexCode;

            if (currentIndexCode != null)
            {
                for (var i = 0; i < data.maps.Count; i++)
                {
                    if (data.maps[i].indexCode == currentIndexCode)
                    {
                        _map.indexCode = currentIndexCode;
                        data.maps[i] = _map;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                indexCode = GenerateIndexCode();
                _map.indexCode = indexCode;
                data.maps.Add(_map);
            }

            IOSaveLoad<SaveData>.SaveData(data, SAVE_NAME);
            completion?.Invoke(indexCode);
        }
        #endregion

        #region Private Methods
        public static IEnumerator GetAllMapsImages(Action<Sprite[]> completion)
        {
            var data = IOSaveLoad<SaveData>.LoadData(SAVE_NAME);

            if (data != null)
            {
                var result = new Sprite[data.maps.Count];
                var imagesCount = data.maps.Count;

                for (var i = 0; i < imagesCount; i++)
                {
                    var text2D = IOSaveLoad<Texture2D>.LoadImage(IMAGE_NAME + data.maps[i].indexCode);
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

        private static string GenerateIndexCode()
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