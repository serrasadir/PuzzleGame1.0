using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlastGame{
    [System.Serializable]
    public class LevelDataJson
    {
        public int level_number;
        public int grid_width;
        public int grid_height;
        public int move_count;
        public List<string> grid;
    }
}