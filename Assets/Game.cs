using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField]
    GameObject tile;

    [SerializeField]
    GameObject emptytile;

    [SerializeField]
    GameObject sheep;

    [SerializeField]
    GameObject AIObject;

    [SerializeField]
    static List<GameObject> board;

    static List<GameObject> AIs;

    Vector3 location = new Vector3(0, 0, 1);
    // Use this for initialization
    void Start()
    {
        board = new List<GameObject>();
        CreateGameBoard();
        Directions.directions = new List<Vector3>();

        Directions.directions.Add(new Vector3(1, 1.75f, 0)); //NE
        Directions.directions.Add(new Vector3(2, 0, 0)); //E
        Directions.directions.Add(new Vector3(1, -1.75f, 0)); //SE
        Directions.directions.Add(new Vector3(-1, -1.75f, 0)); //SW
        Directions.directions.Add(new Vector3(-2, 0f, 0)); //W
        Directions.directions.Add(new Vector3(-1, 1.75f, 0)); //NW

        GameData.init();

        AIs = new List<GameObject>();
        for(byte i = 2; i <= GameData.PlayerCount; i++)
        {
            GameObject CPU = (GameObject)Instantiate(AIObject, Vector3.zero, Quaternion.identity);
            CPU.GetComponent<AI>().SetPlayerID(i);
            board[i * 10].GetComponent<TileData>().SetOwnership(i);
            AIs.Add(CPU);
        }
        board[0].GetComponent<TileData>().SetOwnership(1);
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        GameData.CheckWinner();
    }

    void CreateGameBoard() //More like Game Bored AMIRITE?!?!?
    {
        byte iterator = 0; //0 to 256
        ushort spawn_amount = 64; //0 and 65535
        sbyte x_location = 0; //-128 to 127    
        float y_distance = -1.75f;
        byte x_distance = 2;

        for (ushort i = 0; i < spawn_amount; i++)
        {
            GameObject instancedTile = Instantiate(tile, location, Quaternion.identity);
            instancedTile.GetComponent<TileData>().ID = i;
            board.Add(instancedTile);
            location.x += x_distance;
            iterator++;
            if (iterator == Mathf.Sqrt(spawn_amount))
            {
                iterator = 0;
                if (x_location == 0)
                    x_location = -1;
                else
                    x_location = 0;
                location.x = x_location;
                location.y -= y_distance;
            }
        }
    }

    void StartGame()
    {
        UpdateTileValues();
        GameData.NextTurn();
    }

    public static void UpdateTileValues()
    {
        foreach(GameObject tile in board)
        {
            TileData d = tile.GetComponent<TileData>();
            d.TileValue = d.GetValue();
        }
    }

    public static List<GameObject> GetBoard()
    {
        return board;
    }

    public static List<GameObject> GetCPUs()
    {
        return AIs;
    }
}
