using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



    public enum HouseType
    {
        CHURCH,
        WELL,
        HOUSE1,
        HOUSE2
    }

    [SerializeField]
    GameObject[] housesPrefab;

    float[,] perlinValues;
    [SerializeField]
    float perlinScale = 5.0f;
	// Use this for initialization
	void Start () {
        screenSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize) * 2;
        tileSizeX = housesPrefab[(int)HouseType.CHURCH].GetComponent<SpriteRenderer>().bounds.size.x;
        tileSizeY = housesPrefab[(int)HouseType.CHURCH].GetComponent<SpriteRenderer>().bounds.size.y;
        tileNumberX = (int)(screenSize.x/tileSizeX*worldSizeX);
        tileNumberY = (int)(screenSize.y / tileSizeY * worldSizeY);

        perlinValues = new float[tileNumberX, tileNumberY];
        for(int x = 0; x < tileNumberX;x++)
        {
            for(int y = 0; y<tileNumberY;y++)
            {
                perlinValues[x, y] = Mathf.PerlinNoise(
                    (float)x / tileNumberX * perlinScale,
                    (float)y / tileNumberY * perlinScale);

            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
