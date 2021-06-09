using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMap {
    [Serializable]
    public class SaveData {
        public List<MapData> maps;
    }

    [Serializable]
    public class MapData {
        public string indexCode;
        public MapNodeData[,] mapNodes;
    }

    [Serializable]
    public class MapNodeData {
        public MapNode.NodeType currentType;
    }
}
