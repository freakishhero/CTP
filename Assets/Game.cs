using System;
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
        //Create the game board
        board = new List<GameObject>();
        CreateGameBoard();

        //Populate the directions list
        Directions.directions = new List<Vector3>();
        Directions.directions.Add(new Vector3(1, 1.75f, 0)); //NE
        Directions.directions.Add(new Vector3(2, 0, 0)); //E
        Directions.directions.Add(new Vector3(1, -1.75f, 0)); //SE
        Directions.directions.Add(new Vector3(-1, -1.75f, 0)); //SW
        Directions.directions.Add(new Vector3(-2, 0f, 0)); //W
        Directions.directions.Add(new Vector3(-1, 1.75f, 0)); //NW

        //Initialise game data
        GameData.init();

        //initialise the AI players
        AIs = new List<GameObject>();
        for(byte i = 2; i <= GameData.PlayerCount; i++)
        {
            GameObject CPU = (GameObject)Instantiate(AIObject, Vector3.zero, Quaternion.identity);
            CPU.GetComponent<AI>().SetPlayerID(i); 
            AIs.Add(CPU);
        }

        InitialiseGame();
        StartGame();
    }

    void InitialiseGame()
    {
        List<GameObject> border_tiles;

        //Determine which tiles are on the border of the grid
        border_tiles = new List<GameObject>();
        foreach (GameObject tile in board)
        {
            if (tile.GetComponent<TileData>().GetSurroundingEmptyTiles() <= 4)
            {
                border_tiles.Add(tile);
            }
        }

        //Decide a random number of tiles to remove to create a "random" board
        int removed_tiles = (int)UnityEngine.Random.Range(1, border_tiles.Count - 1);

        //Remove tiles
        for (int i = 0; i < removed_tiles; i++)
        {
            int index = UnityEngine.Random.Range(1, border_tiles.Count);
            index--;
            if (!border_tiles[index].GetComponent<TileData>().RemoveTile)
                border_tiles[index].GetComponent<TileData>().RemoveTile = true;
        }

        //Refresh border_tiles
        border_tiles.Clear();


        //redetermines border tiles, excluding removed tiles
        foreach (GameObject tile in board)
        {
            int empty_tiles = tile.GetComponent<TileData>().GetSurroundingEmptyTiles();
            bool removed = tile.GetComponent<TileData>().RemoveTile;
            int id = tile.GetComponent<TileData>().ID;
            if (empty_tiles <= 4 && !removed)
            {
                border_tiles.Add(tile);
            }
        }

        //Create a start location for the AI on the edge of the grid
        foreach (GameObject cpu in AIs)
        {
            int index = UnityEngine.Random.Range(1, border_tiles.Count);
            index--;
            border_tiles[index].GetComponent<TileData>().SetOwnership(cpu.GetComponent<AI>().GetPlayerID());
            border_tiles.Remove(border_tiles[index]);
        }

        //Create a start location for the plaer on the edge of the grid
        border_tiles[UnityEngine.Random.Range(0, border_tiles.Count - 1)].GetComponent<TileData>().SetOwnership(1);
        border_tiles.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        //See if the game is over
        if(!GameData.GameOver)
            GameData.CheckWinner();
    }

    void CreateGameBoard()
    {
        byte iterator = 0; //0 to 256
        ushort spawn_amount = 64; //0 and 65535
        sbyte x_location = 0; //-128 to 127    
        float y_distance = -1.75f;
        byte x_distance = 2;

        //Creates a grid
        for (ushort i = 0; i < spawn_amount; i++)
        {
            //creates a tile and adds it to the game board list
            GameObject instancedTile = Instantiate(tile, location, Quaternion.identity);
            instancedTile.GetComponent<TileData>().ID = i;
            board.Add(instancedTile);
            location.x += x_distance;
            iterator++;

            //Creates rows relative to the size of the grid
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
        //Update tiles and start game
        UpdateTileValues();
        GameData.NextTurn();
    }

    public static void UpdateTileValues()
    {
        foreach(GameObject tile in board)
        {
            //Gets the worth of each tile
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
