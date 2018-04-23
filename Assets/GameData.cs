using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    static int playersTurn;
    static int playerCount;
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
                Game.getCPUs()[GameData.PlayersTurn - 1].GetComponent<AI>().setTurnState(false);

            PlayersTurn++;
        }
        else
        {
            PlayersTurn = 1;
        }
        Debug.Log("Player " + PlayersTurn + "'s turn.");
    }
}
