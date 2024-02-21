using UnityEngine;
using System.Collections;

namespace BlastGame
{
    public class CubeMover : MonoBehaviour
    {
        public GameManager grid;
        public float lowerAmount = 1.2f;
        private int LowestNull(int x)
        {
            for (int y = 0; y < grid.height ; y++) // start from the bottom triesfind the first nullcell -> itisthe lowest null cell sincewe started from the bottom
            {
                if (grid.grid[x, y].cube == null) // find the lowest empty cell to know where should we shift snd then spawn the new cube
                {
                    return y;
                }
            }
            return -1;
        }
        void SpawnNewCube(int x)//spawn at the top of the grid then move it to the lower cells
        {
            float spacing = 0.5f;
            float xOffset = (grid.width - 1) * spacing / 2;
            float yOffset = (grid.height) * spacing / 2;

            int lowestNullY = LowestNull(x);

            if (lowestNullY != -1)
            {
                Vector3 spawnPosition = new Vector3(x * spacing - xOffset, ((grid.height) * spacing - yOffset) - lowerAmount, 0); 

                int randInd = Random.Range(0, grid.cubePrefabs.Length - 4); //decide the cubes color randomly, but the type is normal
                GameObject newCube = Instantiate(grid.cubePrefabs[randInd], spawnPosition, Quaternion.identity);
                newCube.transform.parent = grid.gridGO.transform;

                newCube.GetComponent<Cube>().SetIndices(x, grid.height);
                grid.grid[x, lowestNullY] = new GridNode(true, newCube);

                Vector3 targetPosition = new Vector3(x * spacing - xOffset, (lowestNullY * spacing - yOffset) - lowerAmount, 0);

                newCube.GetComponent<Cube>().MoveToTargetPos(targetPosition);
            }
        }

        public void ShiftCubesDown(int x, int initialY) 
        {
            float spacing = 0.5f;

            for (int y = initialY + 1; y < grid.height; y++)
            {
                if (grid.grid[x, y].cube != null)
                {
                    Cube cubeAbove = grid.grid[x, y].cube.GetComponent<Cube>();
                    Vector3 targetPos = new Vector3(x * spacing - (grid.width - 1) * spacing / 2, (y - 1) * spacing - (grid.height) * spacing / 2 - lowerAmount, 0);
                    cubeAbove.MoveToTargetPos(targetPos); //get the above cube and move it to the empty space

                    grid.grid[x, y - 1] = grid.grid[x, y];
                    grid.grid[x, y] = new GridNode(true, null); //since we move the cube that suppose to be here, now this cell is empty-null

                    cubeAbove.SetIndices(x, y - 1);
                }
            }

            SpawnNewCube(x);//we need to spawn new cubes to fill the empty columns
        }
    }
}