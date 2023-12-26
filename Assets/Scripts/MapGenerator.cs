using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Map
    {
        public Vector2 mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;

        public float minObstacleHeight;
        public float maxObstacleHeight;

        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter
        {
            get
            {
                return new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);
            }
        }
    }

    [System.Serializable]
    public struct Coord
    {
        public int x, y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord a, Coord b)
        {
            return a.x == a.y && b.x == b.y;
        }
        public static bool operator !=(Coord a, Coord b)
        {
            return !(a == b);
        }
    }

    Player player;
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform navMeshMask;
    public Transform mapFloor;
    public Vector2 maxMapSize;

    [Range(0, 1)]
    public float outlinePercent;

    public float tileSize;

    Coord mapCenter;
    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoord;
    Queue<Coord> shuffledOpenTileCoord;
    Transform[,] tileMap;

    public Map[] maps;
    public int mapIndex;
    Map currentMap;

    private void Start()
    {
        //GenerateMap();
        player = FindObjectOfType<Player>();
    }

    public void GenerateMap(int generateMapIndex=-1)
    {
        if (maps.Length <= 0)
            return;
        if (generateMapIndex >= 0 && generateMapIndex < maps.Length)
        {
            mapIndex = generateMapIndex;
        }
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        allTileCoords = new List<Coord>();

        for(int i = 0; i < currentMap.mapSize.x; i++)
        {
            for(int j = 0; j < currentMap.mapSize.y; j++)
            {
                allTileCoords.Add(new Coord(i, j));
            }
        }

        mapCenter = new Coord((int)(currentMap.mapSize.x / 2f), (int)(currentMap.mapSize.y / 2f));
        tileMap = new Transform[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        // generatint parent
        shuffledTileCoord = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        string parentName = "Generated Map";
        if (transform.Find(parentName) != null)
            DestroyImmediate(transform.Find(parentName).gameObject);

        Transform mapHolder = new GameObject(parentName).transform;
        mapHolder.parent = transform;

        // generatint tile
        for (int i = 0; i < currentMap.mapSize.x; i++)
        {
            for (int j = 0; j < currentMap.mapSize.y; j++)
            {
                Transform tile = Instantiate(tilePrefab, CoordToPosition(i, j), Quaternion.Euler(Vector3.right * 90));
                tile.transform.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                tile.transform.parent = mapHolder;
                tileMap[i, j] = tile;
            }
        }

        // spawnning obstacle
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);
        
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            currentObstacleCount++;
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePos = CoordToPosition(randomCoord.x, randomCoord.y);
                float height = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePos + Vector3.up * height * 0.5f, Quaternion.identity);
                newObstacle.transform.localScale = new Vector3((1 - outlinePercent) * tileSize, height, (1 - outlinePercent) * tileSize);
                newObstacle.parent = mapHolder;

                //Renderer renderer = newObstacle.GetComponent<Renderer>();
                //Material material = new Material(renderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                //material.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                //renderer.sharedMaterial = material;

                //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                //newObstacle.GetComponent<MeshRenderer>().GetPropertyBlock(mpb);
                //mpb.SetColor("_Color", Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent));
                //newObstacle.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                currentObstacleCount--;
                obstacleMap[randomCoord.x, randomCoord.y] = false;
            }
        }

        shuffledOpenTileCoord = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;

        Vector3 maskPosOffset = navMeshMask.position;

        // generating nav mash mask
        Transform maskLeft = Instantiate(navMeshMask, (maxMapSize.x + currentMap.mapSize.x) / 4f * Vector3.left * tileSize + maskPosOffset, Quaternion.Euler(Vector3.right * 90));
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navMeshMask, (maxMapSize.x + currentMap.mapSize.x) / 4f * Vector3.right * tileSize + maskPosOffset, Quaternion.Euler(Vector3.right * 90));
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navMeshMask, (maxMapSize.y + currentMap.mapSize.y) / 4f * Vector3.forward * tileSize + maskPosOffset, Quaternion.Euler(Vector3.right * 90));
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navMeshMask, (maxMapSize.y + currentMap.mapSize.y) / 4f * Vector3.back * tileSize + maskPosOffset, Quaternion.Euler(Vector3.right * 90));
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        obstacleMap[mapCenter.x, mapCenter.y] = true;
        queue.Enqueue(mapCenter);
        int accessibleTileCount = 1;

        Coord tile;
        while(queue.Count > 0)
        {
            tile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 || y == 0)
                    {
                        int neighbourX = tile.x + x;
                        int neighbourY = tile.y + y;
                        if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessbleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessbleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Transform GetTileByPos(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / tileSize - 0.5f + currentMap.mapSize.x / 2f);
        int y = Mathf.RoundToInt(pos.z / tileSize - 0.5f + currentMap.mapSize.y / 2f);

        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);

        return tileMap[x, y];

    }

    public Coord GetRandomCoord()
    {
        Coord item = shuffledTileCoord.Dequeue();
        shuffledTileCoord.Enqueue(item);
        return item;
    }

    public Transform GetRandomOpenTile()
    {
        Coord t = shuffledOpenTileCoord.Dequeue();
        shuffledOpenTileCoord.Enqueue(t);
        return tileMap[t.x, t.y];
    }
}
