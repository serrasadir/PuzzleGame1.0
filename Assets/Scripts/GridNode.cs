using UnityEngine;
using System.Collections;
namespace BlastGame
{
    public class GridNode : MonoBehaviour
    {
        public bool isOccupied;
        public GameObject cube;

        public GridNode(bool _occupied, GameObject _cube)
        {
            isOccupied = _occupied;
            cube = _cube;
        }
    }
}