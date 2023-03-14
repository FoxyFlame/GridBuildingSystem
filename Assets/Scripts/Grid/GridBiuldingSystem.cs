using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GridBiuldingSystem : MonoBehaviour
{
    [SerializeField] private PlacedObjectTypeSizeAndOrientation[] objectToPlace;

    private GridXZ<GridObject> grid;

    static int gridWidth = 10;
    static int gridLength = 10;
    int cellSize = 10;

    bool[,] gridChecker = new bool[gridWidth, gridLength];

    private void Awake()
    { 
        grid = new GridXZ<GridObject>(gridWidth, gridLength, cellSize, transform.position, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        for(int X=0; X<gridWidth; X++)
        {
            for(int Z=0; Z<gridLength; Z++)
            {
                gridChecker[X, Z] = true;
            }
        }
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private Transform transform;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void ClearTransform()
        {
            transform = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return transform == null;
        }

        public override string ToString()
        {
            return x + ":" + z;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach(var gridCheck in gridChecker)
            {
                if(gridCheck)
                {
                    PlaceObject();
                }
            }
        }
    }

    private void PlaceObject()
    {
        int width = Random.Range(0, gridWidth);
        int length = Random.Range(0, gridLength);
        int randomModel = Random.Range(0, objectToPlace.Count());

        if ((width + objectToPlace[randomModel].width - 1) >= gridWidth || (length + objectToPlace[randomModel].length - 1) >= gridLength || width < 0 || length < 0)
        {
            PlaceObject();
        }
        else
        {
            List<Vector2Int> gridPositionList = objectToPlace[randomModel].GetGridPositionList(new Vector2Int(width, length), PlacedObjectTypeSizeAndOrientation.Dir.Down);

            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                Transform buildTransform = Instantiate(objectToPlace[randomModel].prefab, grid.GetWorldPosition(width, length), Quaternion.identity);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    gridChecker[gridPosition.x, gridPosition.y] = false;
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(buildTransform);
                }
            }
            else
            {
                PlaceObject();
            }
        }
    }
    /*
    private void PlaceObject2()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int width, out int length);
            if((width + objectToPlace.width - 1) >= gridWidth || (length + objectToPlace.length - 1) >= gridLength || width < 0 || length < 0)
            {
                Debug.Log("You cannot build here!" + Mouse3D.GetMouseWorldPosition());
            }
            else
            {
                List<Vector2Int> gridPositionList = objectToPlace.GetGridPositionList(new Vector2Int(width, length), PlacedObjectTypeSizeAndOrientation.Dir.Down);

                bool canBuild = true;
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                    {
                        canBuild = false;
                        break;
                    }
                }

                if (canBuild)
                {
                    Transform buildTransform = Instantiate(objectToPlace.prefab, grid.GetWorldPosition(width, length), Quaternion.identity);

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(buildTransform);
                    }
                }
                else
                {
                    Debug.Log("You cannot build here!" + Mouse3D.GetMouseWorldPosition());
                }
            }
        }
    }*/
}
