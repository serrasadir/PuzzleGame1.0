using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BlastGame
{
    public class GameManager : MonoBehaviour
    {
        public int height;
        public int width;
        public int moveCount;

        public int boxCount;
        public int vaseCount;
        public int stoneCount;

        public GameObject[] cubePrefabs;
        public GridNode[,] grid;
        public GameObject gridGO;

        public static GameManager Instance { get; private set; }

        public bool isWin;
        public bool isLevelCompleted = false;

        public EndLevelHandler endLevelHandler;
        public LevelLoadFromJson levelLoader;

        public GameObject LevelPassedOverlay;
        public GameObject LevelFailedOverlay;

        public int currentLevel;
        public float lowerAmount = 1.0f;

        public LevelDataJson currentLevelData;
        public CubeMover cubeMover;

        HashSet<Cube> markedCubes ;
        HashSet<Cube> markedVases ;

        private void Awake()
        {

            Instance = this;
        }

        void Start()
        {
            if (levelLoader != null)
            {
                LevelPassedOverlay.SetActive(false);
                LevelFailedOverlay.SetActive(false); //close the overlays, they made for the end of the level
                currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1); //determine the level
                levelLoader.LoadLevel(currentLevel);//load the level data from json

            }
            else
            {
                Debug.LogError("LevelLoader is not assigned");
            }
        }

        public int MoveCount
        {
            get { return moveCount; }
            set
            {
                moveCount = value;
            }
        }

        public void InitializeGridFromLevelData()
        {
            markedVases = new HashSet<Cube>();
            markedCubes = new HashSet<Cube>();
            isLevelCompleted = false;
            width = currentLevelData.grid_width; //load these from the json via LevelLoadFromJson
            height = currentLevelData.grid_height;
            moveCount = currentLevelData.move_count;
            Debug.Log($"Move count: {moveCount}");

            grid = new GridNode[width, height];

            float spacing = 0.5f;
            float xOffset = (width - 1) * spacing / 2;
            float yOffset = (height) * spacing / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    Vector2 position = new Vector2(x * spacing - xOffset,( y * spacing - yOffset) - lowerAmount);
                    string cubeType = currentLevelData.grid[index];
                    GameObject cube = null;
                    int prefabIndex = GetPrefabIndex(cubeType); //to specify which cube type is exracted from the json and use the prefab of it

                    if (prefabIndex != -1) //error check
                    {
                        cube = Instantiate(cubePrefabs[prefabIndex], position, Quaternion.identity);
                        cube.transform.parent = gridGO.transform;
                        cube.GetComponent<Cube>().SetIndices(x, y);
                        grid[x, y] = new GridNode(cubeType != "empty", cube);
                    }
                }
            }
            FindObjectOfType<GoalPanel>().UpdateVaseCount(vaseCount); //update the obstacle counters in top up
            FindObjectOfType<GoalPanel>().UpdateBoxCount(boxCount);
            FindObjectOfType<GoalPanel>().UpdateStoneCount(stoneCount);
            CheckForInitialMatches();
        }

        void CheckForInitialMatches()//after every move we need to check the bomb hints -> whether connectedcubes >= 5 or not
        {                             //and adjust the sprite and type of the cube according to it
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    bool[,] visitedForTNT = new bool[width, height];

                    Cube cubeForTNT = grid[x, y].cube.GetComponent<Cube>();
                    if (cubeForTNT != null)
                    {
                        List<Cube> connectedCubesForTNT = FindConnectedCubes(cubeForTNT.xPos, cubeForTNT.yPos, cubeForTNT.cubeColor, visitedForTNT);
                        if (connectedCubesForTNT.Count >= 5)
                        {
                            foreach (Cube cubeTNT in connectedCubesForTNT)
                            {

                                cubeTNT.GetComponent<Cube>().ChangeToTntHint(); //if onnectedcubes >= 5 , change the sprite and the type
                            }
                        }
                        else
                        {
                            foreach (Cube cubeNormal in connectedCubesForTNT)
                            {
                                cubeNormal.GetComponent<Cube>().ChangeToNormal(); //else convert them into normal cubes
                            }
                        }
                    }
                }
            }
        }

        int GetPrefabIndex(string cubeType) //to decide which prefab is exracted from the json
        {
            switch (cubeType)
            {
                case "r": return 0;
                case "g": return 1;
                case "b": return 2;
                case "y": return 3;
                case "t": return 4;
                case "bo": //if any obstacle is created, increase the counter of that obstacle
                    boxCount++;
                    return 5;
                case "s":
                    stoneCount++;
                    return 6;
                case "v":
                    vaseCount++;
                    return 7;
                case "rand":
                    return Random.Range(0, 3);
                default: return -1;
            }
        }

        List<Cube> FindConnectedCubes(int x, int y, CubeColor color, bool[,] visited) //how many same colored cubes are adjacent? 
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return new List<Cube>(); //bounds check
            if (grid[x, y].cube == null || visited[x, y] || grid[x, y].cube.GetComponent<Cube>().cubeColor != color) return new List<Cube>();

            visited[x, y] = true;
            List<Cube> connected = new List<Cube> { grid[x, y].cube.GetComponent<Cube>() };

            connected.AddRange(FindConnectedCubes(x + 1, y, color, visited)); //right
            connected.AddRange(FindConnectedCubes(x - 1, y, color, visited)); //left
            connected.AddRange(FindConnectedCubes(x, y + 1, color, visited)); //up
            connected.AddRange(FindConnectedCubes(x, y - 1, color, visited)); //dpwn

            return connected;
        }

        void MarkObstaclesToDestroy(int x, int y) //check for the adjacent obstacles to destrot them later
        {
            int[] dx = { -1, 1, 0, 0 }; // check the adjacent left,right,down,up cubes toseeif they are obstacles
            int[] dy = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int adjX = x + dx[i];
                int adjY = y + dy[i];
                if (adjX >= 0 && adjX < width && adjY >= 0 && adjY < height)
                {
                    if (grid[adjX, adjY].cube != null)
                    {
                        Cube adjCube = grid[adjX, adjY].cube.GetComponent<Cube>();
                        if (adjCube != null && adjCube.isObstacle)
                        {
                            Debug.Log($"{markedVases}");
                            if (!markedVases.Contains(adjCube) && adjCube.cubeColor == CubeColor.Vase) //vase got the first damage, convert it to a broken one
                            {
                                markedCubes.Add(adjCube);
                                markedVases.Add(adjCube);
                            }
                            else if (!markedCubes.Contains(adjCube) && adjCube.cubeColor != CubeColor.Vase && adjCube.cubeColor != CubeColor.Stone)//stones can only damaged by tnt so dont mark it                                               
                            {                                                                                                                   //vase have to 2 rights to get damage if it is not broken dont mark it
                                markedCubes.Add(adjCube);
                            }
                        }
                    }
                }
            }
        }

        void MarkCubesToDestroy(List<Cube> cubes, ref Cube cubeClicked) 
        {
            int xCenter = cubeClicked.xPos;
            int yCenter = cubeClicked.yPos;
            HashSet<Cube> alreadyExploded = new HashSet<Cube>();//prevent the nested loop due to explodeTNT() function

            if (cubeClicked.cubeColor == CubeColor.TNT)
            {
                ExplodeTNT(cubeClicked, cubes, xCenter, yCenter, alreadyExploded);
            }
            else if (cubeClicked.cubeType == CubeType.Normal)
            {
                foreach (Cube cube in cubes)
                {
                    int x = cube.xPos;
                    int y = cube.yPos;

                    //animation will come here

                    if (x < 0 || x >= width || y < 0 || y > height)
                    {
                        Debug.Log($"Attempting to access out of bounds {x},{y}");
                    }
                    else if (cube.cubeColor == CubeColor.TNT)
                    {
                        ExplodeTNT(cube, cubes, cube.xPos, cube.yPos, alreadyExploded);
                    }
                    else if (!cube.isObstacle && !markedCubes.Contains(cube))
                    {
                        markedCubes.Add(cube);
                        MarkObstaclesToDestroy(x, y);
                    }
                }
            }
            else if (cubeClicked.cubeType == CubeType.Tnt_hint)
            {
                foreach (Cube cube in cubes)
                {
                    cubeClicked.ChangeToTNTAbout();
                    int x = cube.xPos;
                    int y = cube.yPos;
                    if (x < 0 || x >= width || y < 0 || y >= height)
                    {
                        Debug.Log($"Attempting to access out ofbounds {x},{y}");
                    }

                    else if (cube.cubeColor == CubeColor.TNT)
                    {
                        ExplodeTNT(cube, cubes, xCenter, yCenter, alreadyExploded);
                    }
                    else if (cube.cubeType == CubeType.Tnt_hint && !markedCubes.Contains(cube))
                    {
                        MarkObstaclesToDestroy(x,y);
                        markedCubes.Add(cube);
                    }
                    else if (cube.cubeType == CubeType.TNT_about_to_convert && !markedCubes.Contains(cube))
                    {
                        MarkObstaclesToDestroy(x,y);
                        cube.ChangeToTNT();
                    }
                }
            }
            DestroyMarkedCubes();
        }

        void DestroyMarkedCubes()//now we can destroy all the marked cubes - except the ubroken vases of course
        {
            foreach (Cube cube in markedCubes)
            {
                if (cube != null && cube.cubeColor != CubeColor.Vase)
                {
                    if (cube.isObstacle)
                    {
                        switch (cube.cubeColor)
                        {
                            case CubeColor.Box: //update the obstacle counters
                                boxCount--;
                                FindObjectOfType<GoalPanel>().UpdateBoxCount(boxCount);
                                break;
                            case CubeColor.Stone:
                                stoneCount--;
                                FindObjectOfType<GoalPanel>().UpdateStoneCount(stoneCount);
                                break;
                            case CubeColor.BrokenVase:
                                vaseCount--;
                                FindObjectOfType<GoalPanel>().UpdateVaseCount(vaseCount);
                                break;
                        }
                    }
                    int x = cube.xPos;
                    int y = cube.yPos;
                    if (x < 0 || x >= width || y < 0 || y >= height)
                    {
                        Debug.Log($"Attempting to access out of bounds {x},{y}");
                    }
                    else
                    {

                        GamePlayAnimations popAnimation = cube.GetComponent<GamePlayAnimations>(); //destroy the cube, set the grid null for that index
                        popAnimation.PopAnimation(cube.cubeColor);
                        grid[x, y] = new GridNode(true, null);
                        cubeMover.ShiftCubesDown(x, y); //fill the empty space by moving the above 
                    }
                }
                else if(cube.cubeColor == CubeColor.Vase)
                {
                    cube.ChangeToBrokenVase();
                }

            }
            markedVases.Clear();
            markedCubes.Clear();
        }

        void ExplodeTNT(Cube cubeTNT, List<Cube> cubes, int xCenter, int yCenter, HashSet<Cube> alreadyExploded) 
        {
            if (!alreadyExploded.Add(cubeTNT)) { return; }
           
            int radius = cubes.Count == 1 ? 2 : 3; // 5x5 or 7x7?

            markedCubes.Add(cubeTNT);
                
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    int xTarget = xCenter + i;
                    int yTarget = yCenter + j;

                    if (xTarget >= 0 && xTarget < width && yTarget >= 0 && yTarget < height && grid[xTarget, yTarget].cube != null)
                    {
                        Cube targetCube = grid[xTarget, yTarget].cube.GetComponent<Cube>();
                        if (!targetCube.isObstacle) { MarkObstaclesToDestroy(xTarget, yTarget); } //if  it is already an obstacle dont check for the adjacent obstacles
                        if (targetCube.cubeColor == CubeColor.TNT && alreadyExploded.Contains(targetCube))//if it is an tnt, trigger this explosion too
                        {
                            bool[,] visited = new bool[width, height];
                            List<Cube> newCubes = FindConnectedCubes(targetCube.xPos, targetCube.yPos, targetCube.cubeColor, visited);
                            ExplodeTNT(targetCube, newCubes, targetCube.xPos, targetCube.yPos, alreadyExploded);
                        }
                        else
                        {
                            if(!markedCubes.Contains(targetCube) && (targetCube.cubeColor != CubeColor.Vase )) //if it a stone or a normal cube mark to destroy
                            {
                                markedCubes.Add(targetCube);
                            }
                            else if(!markedCubes.Contains(targetCube) && targetCube.cubeColor == CubeColor.Vase)
                            {
                                targetCube.ChangeToBrokenVase();
                                markedVases.Add(targetCube);
                            }
                        }
                    }
                }
            }
        }
        

        public void OnCubeClicked(Cube cube)
        {
            if (!isLevelCompleted)
            {
                if (cube.isObstacle)
                {
                    Debug.Log($"cannot click on a obstacle {cube.cubeType}");
                }
                else
                {
                    Debug.Log($"Clicked {cube.xPos},{cube.yPos}");
                    bool[,] visited = new bool[width, height];
                    List<Cube> connectedCubes = FindConnectedCubes(cube.xPos, cube.yPos, cube.cubeColor, visited);
                    if (cube.cubeColor == CubeColor.TNT)
                    {
                        moveCount--;
                        FindObjectOfType<GoalPanel>().UpdateMoveCount(MoveCount);
                        MarkCubesToDestroy(connectedCubes, ref cube);
                    }
                    else if (connectedCubes.Count >= 2) //if the cube is not a tnt  there sould  be at least 2 connected same colored cubes
                    {
                        moveCount--;
                        FindObjectOfType<GoalPanel>().UpdateMoveCount(MoveCount);
                        MarkCubesToDestroy(connectedCubes, ref cube);
                    }
                    if (vaseCount <= 0 && boxCount <= 0 && stoneCount <= 0 && moveCount >= 0) //all the obstacles has broken before the move counter has finished (win situation)
                    {
                        isWin = true;
                        isLevelCompleted = true;
                        endLevelHandler.HandleWinCondition(); //do the celebration animation and go back to the main page 
                    }
                    else if (moveCount <= 0 && !isWin) //cannot broke the obstacles in limited moves (failed the level)
                    {
                        isWin = false;
                        isLevelCompleted = true;
                        endLevelHandler.HandleFailCondition();//go back to the main page 
                    }
                }
                CheckForInitialMatches();
            }
            else
            {
                Debug.Log("game  over");
            }
        }
    }
}
