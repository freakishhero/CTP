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
        createGameBoard();
        GameData.PlayerCount = 4;
        GameData.PlayersTurn = 1;

        AIs = new List<GameObject>();
        for(byte i = 2; i <= GameData.PlayerCount; i++)
        {
            GameObject CPU = (GameObject)Instantiate(AIObject, Vector3.zero, Quaternion.identity);
            CPU.GetComponent<AI>().setPlayerID(i);
            board[i * 10].GetComponent<TileData>().setOwnership(i);
            AIs.Add(CPU);
        }
        board[0].GetComponent<TileData>().setOwnership(1);

        /* board[0].GetComponent<TileData>().setOwnership(1);
         board[5].GetComponent<TileData>().setOwnership(2);
         board[60].GetComponent<TileData>().setOwnership(3);
         board[64].GetComponent<TileData>().setOwnership(4);
         board[146].GetComponent<TileData>().RemoveTile = true;
         board[109].GetComponent<TileData>().RemoveTile = true;
         board[36].GetComponent<TileData>().RemoveTile = true;
         board[37].GetComponent<TileData>().RemoveTile = true;
         board[34].GetComponent<TileData>().RemoveTile = true;
         board[108].GetComponent<TileData>().RemoveTile = true;
         board[123].GetComponent<TileData>().RemoveTile = true;
         board[161].GetComponent<TileData>().RemoveTile = true;*/


        //board[0].GetComponent<TileData>().setOwnership(1);
       board[31].GetComponent<TileData>().RemoveTile = true;
        board[37].GetComponent<TileData>().RemoveTile = true;
        board[17].GetComponent<TileData>().RemoveTile = true;
        board[18].GetComponent<TileData>().RemoveTile = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        //PROCESSAI
        //PROECSSPLAYER
    }

    void createGameBoard() //More like Game Bored AMIRITE?!?!?
    {
        byte iterator = 0; //0 to 256
        ushort spawn_amount = 225; //0 and 65535
        sbyte x_location = 0; //-128 to 127    
        float y_distance = -1.75f;
        byte x_distance = 2;

        for (ushort i = 0; i < spawn_amount; i++)
        {
            GameObject instancedTile = (GameObject)Instantiate(tile, location, Quaternion.identity);
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

    public static List<GameObject> getBoard()
    {
        return board;
    }

    public static List<GameObject> getCPUs()
    {
        return AIs;
    }
}
