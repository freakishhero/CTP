using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    static int playersTurn;
    static int playerCount;
    static int turns;

    // Use this for initialization
    void Start()
    { 
	}

    public static int PlayersTurn
    {
        get
        {
            return playersTurn;
        }
        set
        {
            playersTurn = value;
        }
    }

    public static int Turns
    {
        get
        {
            return turns;
        }
        set
        {
            turns = value;
        }
    }

    public static int PlayerCount
    {
        get
        {
            return playerCount;
        }
        set
        {
            playerCount = value;
        }
    }

    public static void EndTurn()
    {
        if (PlayersTurn < 4)
        {
            if (PlayersTurn > 0 && PlayersTurn < 4)
                Game.GetCPUs()[GameData.PlayersTurn - 1].GetComponent<AI>().setTurnState(false);

            PlayersTurn++;
        }
        else
        {
            PlayersTurn = 1;
        }
        turns++;
        Debug.Log("Turn " + GameData.Turns + ": player " + GameData.PlayersTurn + "'s turn!");
        Game.UpdateTileValues();
        
    }
}
