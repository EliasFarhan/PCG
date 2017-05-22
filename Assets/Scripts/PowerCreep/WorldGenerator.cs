using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePosComp : IComparer<TilePos>
{
    // Compares by Height, Length, and Width.
    public int Compare(TilePos n1, TilePos n2)
    {
        float d1 = n1.GetDistanceToTarget();
        float d2 = n2.GetDistanceToTarget();

        if (d1 < d2)
            return -1;
        else if (d1 > d2)
            return 1;
        return 0;
    }
}

[System.Serializable]
public class TilePos
{
    [SerializeField]
    int posX;
    [SerializeField]
    int posY;
    [SerializeField]
    float perlinValue;
    //For pathfinding
    bool visited = false;
    TilePos parent = null;
    float distanceToTarget = -1.0f;
    TilePos target = null;

    Path path = null;

    public TilePos(int x, int y, float p)
    {
        posX = x;
        posY = y;
        perlinValue = p;
    }

    public float GetPerlinValue()
    {
        return perlinValue;
    }
    public int GetPosX()
    {
        return posX;
    }
    public int GetPosY()
    {
        return posY;
    }
    public void SetPath(Path newPath)
    {
        path = newPath;
    }
    public Path GetPath()
    {
        return path;
    }
    public static float DistanceBetween(TilePos p1, TilePos p2)
    {
        return (new Vector2(p1.GetPosX(), p2.GetPosY())-new Vector2(p2.GetPosX(), p2.GetPosY())).magnitude;
    }
    //Pathfinding
    public void SetVisited()
    {
        visited = true;
    }
    public bool GetVisited()
    {
        return visited;
    }
    public void SetParent(TilePos tile)
    {
        parent = tile;
    }
    public TilePos GetParent()
    {
        return parent;
    }
    public void SetDistanceToTarget(float d)
    {
        distanceToTarget = d;
    }
    public float GetDistanceToTarget()
    {
        return distanceToTarget;
    }
    public void SetTarget(TilePos target)
    {
        this.target = target;
    }
    public TilePos GetTarget()
    {
        return target;
    }
    public void Reset()
    {
        visited = false;
        parent = null;
        distanceToTarget = -1.0f;
    }
    
}

[System.Serializable]
public class City
{
    List<TilePos> cityTiles;
    [SerializeField]
    TilePos center = null;
    [SerializeField]
    int citySize;
    List<City> connectedCity = new List<City>();
    public City(List<TilePos> tiles)
    {
        cityTiles = tiles;
        citySize = cityTiles.Count;
        CalcCenter();
    }

    public bool tileInCity(TilePos pos)
    {
        return cityTiles.Contains(pos);
    }
    public static float DistanceBetween(City c1, City c2)
    {
        return TilePos.DistanceBetween(c1.GetCityCenter(), c2.GetCityCenter());
    }
    public TilePos GetTilePosAt(int x, int y)
    {

        foreach(var tile in cityTiles)
        {
            if(tile.GetPosX() == x && tile.GetPosY() == y)
            {
                return tile;
            }
        }
        return null;
    }
    public List<TilePos> GetCityTiles()
    {
        return cityTiles;
    }
    public void AddNewConnectedCity(City otherCity)
    {
        connectedCity.Add(otherCity);
    }
    public bool AlreadyConnected(City otherCity)
    {
        return connectedCity.Contains(otherCity);
    }
    void CalcCenter()
    {

        float maxPerlinValue = -1.0f;
        foreach(var tile in cityTiles)
        {
            if(maxPerlinValue < 0.0f || tile.GetPerlinValue() > maxPerlinValue)
            {
                maxPerlinValue = tile.GetPerlinValue();
                center = tile;
            }
        }
    }

    public TilePos GetCityCenter()
    {
        return center;
    }



}

public class Path
{
    TilePos currentPos;
    List<TilePos> neighbors;

    WorldGenerator.PathType pathType = WorldGenerator.PathType.NONE;

    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public Path(TilePos current, List<TilePos> neighbors)
    {
        currentPos = current;
        this.neighbors = neighbors;
    }

    public void AddNeighbor(TilePos neighbor)
    {
        if(!neighbors.Contains(neighbor))
            neighbors.Add(neighbor);
    }
    public void CalcPathType()
    {
        List<Direction> dirs = new List<Direction>();
        foreach (var neighbor in neighbors)
        {
            if (neighbor.GetPosX() - currentPos.GetPosX() == 1)
            {
                dirs.Add(Direction.RIGHT);
            }
            if (neighbor.GetPosX() - currentPos.GetPosX() == -1)
            {
                dirs.Add(Direction.LEFT);
            }
            if (neighbor.GetPosY() - currentPos.GetPosY() == 1)
            {
                dirs.Add(Direction.UP);
            }
            if (neighbor.GetPosY() - currentPos.GetPosY() == -1)
            {
                dirs.Add(Direction.DOWN);
            }
        }
        switch (dirs.Count)
        {
            case 4:
                pathType = WorldGenerator.PathType.CROSS;
                break;
            case 3:
                if (!dirs.Contains(Direction.DOWN))
                    pathType = WorldGenerator.PathType.T_NOT_DOWN;
                if (!dirs.Contains(Direction.UP))
                    pathType = WorldGenerator.PathType.T_NOT_UP;
                if (!dirs.Contains(Direction.LEFT))
                    pathType = WorldGenerator.PathType.T_NOT_LEFT;
                if (!dirs.Contains(Direction.RIGHT))
                    pathType = WorldGenerator.PathType.T_NOT_RIGHT;
                break;
            case 2:
                if (dirs.Contains(Direction.DOWN) && dirs.Contains(Direction.UP))
                    pathType = WorldGenerator.PathType.VERTICAL;
                if (dirs.Contains(Direction.DOWN) && dirs.Contains(Direction.RIGHT))
                    pathType = WorldGenerator.PathType.BOTTOM_RIGHT;
                if (dirs.Contains(Direction.DOWN) && dirs.Contains(Direction.LEFT))
                    pathType = WorldGenerator.PathType.BOTTOM_LEFT;
                if (dirs.Contains(Direction.UP) && dirs.Contains(Direction.RIGHT))
                    pathType = WorldGenerator.PathType.TOP_RIGHT;
                if (dirs.Contains(Direction.UP) && dirs.Contains(Direction.LEFT))
                    pathType = WorldGenerator.PathType.TOP_LEFT;
                if (dirs.Contains(Direction.LEFT) && dirs.Contains(Direction.RIGHT))
                    pathType = WorldGenerator.PathType.HORIZONTAL;

                break;
            case 1:
                if (dirs.Contains(Direction.DOWN) || dirs.Contains(Direction.UP))
                    pathType = WorldGenerator.PathType.VERTICAL;
                if (dirs.Contains(Direction.LEFT) || dirs.Contains(Direction.RIGHT))
                    pathType = WorldGenerator.PathType.HORIZONTAL;
                break;

        }
    }
    public WorldGenerator.PathType GetPathType()
    {
        return pathType;
    }
    public TilePos GetTilePos()
    {
        return currentPos;
    }
}

public class WorldGenerator : MonoBehaviour {

    Vector2 screenSize;

    [SerializeField]
    int worldSizeX = 100; //In background value c-a-d 1920
    [SerializeField]
    int worldSizeY = 100; //In background value c-a-d 1080

    float tileSizeX;
    float tileSizeY;

    [SerializeField]
    int tileNumberX;
    [SerializeField]
    int tileNumberY;

    [SerializeField]
    float perlinCityDensityMin = 0.8f;
    [SerializeField]
    float perlinForestDensityMax = 0.2f;
    public enum HouseType
    {
        CHURCH,
        WELL,
        HOUSE1,
        HOUSE2
    }
    public enum PathType
    {
        HORIZONTAL,
        VERTICAL,
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT,
        T_NOT_UP,
        T_NOT_DOWN,
        T_NOT_LEFT,
        T_NOT_RIGHT,
        CROSS,
        NONE

    }

    [SerializeField]
    GameObject[] housesPrefab;
    [SerializeField]
    GameObject forestPrefab;
    [SerializeField]
    GameObject[] pathPrefab;


    TilePos[,] tiles;
    [SerializeField]
    List<City> cities = new List<City>();
    List<Path> paths = new List<Path>();
    [SerializeField]
    float perlinScale = 5.0f;



    // Use this for initialization
    void Start() {
        screenSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize) * 2;
        tileSizeX = housesPrefab[(int)HouseType.CHURCH].GetComponent<SpriteRenderer>().bounds.size.x;
        tileSizeY = housesPrefab[(int)HouseType.CHURCH].GetComponent<SpriteRenderer>().bounds.size.y;
        tileNumberX = (int)(screenSize.x / tileSizeX * worldSizeX);
        tileNumberY = (int)(screenSize.y / tileSizeY * worldSizeY);
        GenerateWorld();
    }
    void GenerateWorld()
    { 
        tiles = new TilePos[tileNumberX, tileNumberY];
        for(int x = 0; x < tileNumberX;x++)
        {
            for(int y = 0; y<tileNumberY;y++)
            {
                tiles[x, y] = new TilePos(x,y,Mathf.PerlinNoise(
                    (float)x / tileNumberX * perlinScale,
                    (float)y / tileNumberY * perlinScale));
                
            }
        }
        //We do BFS on the whole map to generate with BFS again the cities
        TilePos currentTile = tiles[0, 0];
        Queue<TilePos> nextTiles = new Queue<TilePos>();
        while (currentTile != null)
        {

            if (currentTile.GetPerlinValue() > perlinCityDensityMin)
            {
                GenerateNewCity(currentTile);
            }
            else if(currentTile.GetPerlinValue() < perlinForestDensityMax)
            {
                //Instantiate Forest
                GameObject forest = Instantiate(forestPrefab);
                forest.transform.position = new Vector3(
                    tileSizeX*(currentTile.GetPosX()-tileNumberX/2),
                    tileSizeY*(currentTile.GetPosY()-tileNumberY/2),
                    0
                    );
            }
            
            //Adding the two neighbors
            if (currentTile.GetPosX() + 1 != tileNumberX && !nextTiles.Contains(tiles[currentTile.GetPosX() + 1, currentTile.GetPosY()]))
            {
                nextTiles.Enqueue(tiles[currentTile.GetPosX() + 1, currentTile.GetPosY()]);
            }
            if (currentTile.GetPosY() + 1 != tileNumberY && !nextTiles.Contains(tiles[currentTile.GetPosX() , currentTile.GetPosY()+1]))
            {
                nextTiles.Enqueue(tiles[currentTile.GetPosX(), currentTile.GetPosY()+1]);
            }
            if (nextTiles.Count == 0)
            {
                currentTile = null;
            }
            else
            {
                currentTile = nextTiles.Dequeue();
            }
        }
        Debug.Log("City number: " + cities.Count);
        foreach (var city in cities)
        {
           
            //Instantiate the Church
            GameObject church = Instantiate(housesPrefab[(int)HouseType.CHURCH]);
            church.transform.position = new Vector3(
                tileSizeX * (city.GetCityCenter().GetPosX() - tileNumberX / 2),
                tileSizeY * (city.GetCityCenter().GetPosY() - tileNumberY / 2),
                0
                );
            
            
        }
        ConnectCities();
        //Instantiate Houses around the city centers
        foreach(var city in cities)
        {
            foreach(var tile in city.GetCityTiles())
            {
                if(tile != city.GetCityCenter() && tile.GetPath() == null)
                {
                    GameObject house = Instantiate(housesPrefab[Random.Range((int)HouseType.HOUSE1, (int)HouseType.HOUSE2+1)]);
                    house.transform.position = new Vector3(
                        tileSizeX*(tile.GetPosX()-tileNumberX/2),
                        tileSizeY*(tile.GetPosY()-tileNumberY/2),
                        0.0f
                        );
                }

            }
        }
    }

    void GenerateNewCity(TilePos tile)
    {

        //Check if not already a city
        foreach(City city in cities)
        {
            if (city.tileInCity(tile))
                return;
        }
        Debug.Log("Creating New City");
        List<TilePos> newCityTiles = new List<TilePos>();
        //BFS around to get the whole city
        Queue<TilePos> nextTiles = new Queue<TilePos>();
        TilePos currentTile = tile;
        while (currentTile != null)
        {

            //Adding the fours neighbors if they are city tiles
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == j || i == -j)
                        continue;

                    if (currentTile.GetPosX() + i == tileNumberX ||
                        currentTile.GetPosX() + i == -1 || 
                        currentTile.GetPosY() + j == tileNumberY
                        || currentTile.GetPosY() + j == -1)
                        continue;

                    TilePos neighbor = tiles[currentTile.GetPosX() + i, currentTile.GetPosY() + j];
                    if (newCityTiles.Contains(neighbor) || nextTiles.Contains(neighbor))
                        continue;

                    if (neighbor.GetPerlinValue() > perlinCityDensityMin)
                    {
                        nextTiles.Enqueue(neighbor);
                        newCityTiles.Add(neighbor);
                    }
                }
            }
            if (nextTiles.Count == 0)
            {
                currentTile = null;
            }
            else
            {
                currentTile = nextTiles.Dequeue();
            }
        }

        City newCity = new City(newCityTiles);
        cities.Add(newCity);
    }

	void ConnectCities()
    {
        Debug.Log("Connecting Cities");
        foreach(var city in cities)
        {
            //Eight paths around the church
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (city.GetCityCenter().GetPosX() + i == tileNumberX || 
                        city.GetCityCenter().GetPosY() + j == tileNumberY ||
                        city.GetCityCenter().GetPosX() + i == -1 ||
                        city.GetCityCenter().GetPosY() + j == -1 )
                        continue;
                    TilePos neighbor = tiles[city.GetCityCenter().GetPosX() + i, city.GetCityCenter().GetPosY() + j];
                    if (neighbor.GetPath() == null)
                    {
                        Path newPath = new Path(neighbor, new List<TilePos>());
                        neighbor.SetPath(newPath);
                        paths.Add(newPath);
                    }
                    //Adding the neighbors paths
                    if (i == 0)
                    {
                        if (neighbor.GetPosX() - 1 != -1)
                            neighbor.GetPath().AddNeighbor(tiles[neighbor.GetPosX() - 1, neighbor.GetPosY()]);
                        if (neighbor.GetPosX() + 1 != tileNumberX)
                            neighbor.GetPath().AddNeighbor(tiles[neighbor.GetPosX() + 1, neighbor.GetPosY()]);
                    }
                    else if ( j == 0)
                    {
                        if (neighbor.GetPosY() - 1 != -1)
                            neighbor.GetPath().AddNeighbor(tiles[neighbor.GetPosX() , neighbor.GetPosY() - 1]);
                        if (neighbor.GetPosY() + 1 != tileNumberY)
                            neighbor.GetPath().AddNeighbor(tiles[neighbor.GetPosX() , neighbor.GetPosY() + 1]);
                    }
                    else
                    {
                        if (neighbor.GetPosX() - i != tileNumberX &&
                           neighbor.GetPosX() - i != -1 )
                            neighbor.GetPath().AddNeighbor(
                                tiles[neighbor.GetPosX() - i, neighbor.GetPosY()]);
                        if (neighbor.GetPosY() - j != tileNumberY &&
                           neighbor.GetPosY() - j != -1)
                            neighbor.GetPath().AddNeighbor(
                                tiles[neighbor.GetPosX(), neighbor.GetPosY() - j]);
                    }
                }
            }
            float minDistance = -1.0f;
            City closestCity = null;
            foreach(City otherCity in cities)
            {
                if (otherCity == city || otherCity.AlreadyConnected(city))
                    continue;
                if (minDistance < 0.0f || City.DistanceBetween(city, otherCity) < minDistance)
                {
                    closestCity = otherCity;
                    minDistance = City.DistanceBetween(city, otherCity);
                }
            }

            GeneratePath(city, closestCity);
            closestCity.AddNewConnectedCity(city);
            city.AddNewConnectedCity(closestCity);
        }
        //Instantiate Paths
        foreach(Path path in paths)
        {
            path.CalcPathType();
            if (path.GetPathType() != PathType.NONE)
            {
                GameObject pathObject = Instantiate(pathPrefab[(int)path.GetPathType()]);
                pathObject.transform.position = new Vector3(
                    tileSizeX * (path.GetTilePos().GetPosX() - tileNumberX/2),
                    tileSizeY * (path.GetTilePos().GetPosY() - tileNumberY/2),
                    0
                    );
            }
        }
    }

    void GeneratePath(City origin, City target)
    {
        //BFS with priority on direction to target
        Debug.Log("Generating Path");

        Queue<TilePos> nextTilePos = new Queue<TilePos>();

        TilePos originPos = origin.GetCityCenter();
        TilePos targetPos = target.GetCityCenter();

        TilePos currentPos = originPos;
        while(currentPos != targetPos)
        {
            List<TilePos> tmpPos = new List<TilePos>();
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (i == j || i == -j)
                        continue;
                    if (currentPos.GetPosX() + i == -1 ||
                        currentPos.GetPosX() + i == tileNumberX ||
                        currentPos.GetPosY() + j == -1 ||
                        currentPos.GetPosY() + j == tileNumberY)
                        continue;
                    TilePos neighbor = tiles[currentPos.GetPosX() + i,
                        currentPos.GetPosY() + j];
                    //Check if not already visited by other BFS
                    if(neighbor.GetTarget() != targetPos)
                        neighbor.Reset();
                    
                    if (neighbor.GetVisited() || neighbor.GetPerlinValue() < perlinForestDensityMax)
                        continue;

                    neighbor.SetVisited();
                    neighbor.SetTarget(targetPos);
                    neighbor.SetDistanceToTarget(
                        TilePos.DistanceBetween(neighbor, targetPos)
                        );
                    neighbor.SetParent(currentPos);
                    tmpPos.Add(neighbor);
                    
                }
            }
            tmpPos.Sort(new TilePosComp());
            foreach (var neighbor in tmpPos)
            {
                nextTilePos.Enqueue(neighbor);
            }
            //nextTilePos.Sort(new TilePosComp());
            if (nextTilePos.Count == 0)
            {
                break;
            }
            else
            {
                currentPos = nextTilePos.Dequeue();
            }
        }

        //Instantiate Path
        List<TilePos> tilePath = new List<TilePos>();
        while (currentPos != originPos)
        {
            currentPos = currentPos.GetParent();
            tilePath.Add(currentPos);
        }
        tilePath.Reverse();
        for(int i = 1; i < tilePath.Count; i++)
        {
            currentPos = tilePath[i];
            if(currentPos.GetPath() == null)
            {
                Path newPath = new Path(currentPos, new List<TilePos>());
                currentPos.SetPath(newPath);
                paths.Add(newPath);
            }
            if(i > 1)
            {
                currentPos.GetPath().AddNeighbor(tilePath[i - 1]);
            }
            if( i < tilePath.Count - 1)
            {
                currentPos.GetPath().AddNeighbor(tilePath[i + 1]);
            }
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
