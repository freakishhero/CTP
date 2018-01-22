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
}
