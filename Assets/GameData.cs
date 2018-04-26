using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    static int playersTurn;
    static int playerCount;
    static int turns;
    static bool game_over;
    static int[] scores;
    static bool[] can_move;
    static bool speed_increase;
    public static List<int> winners = new List<int>();

    //Use this for initialization
    public static void init()
    {
        //initialise variables
        speed_increase = false;
        playerCount = 4;
        PlayersTurn = 0;
        turns = 0;
        game_over = false;
        scores = new int[4];
        can_move = new bool[4];

        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
        }

        for (int i = 0; i < can_move.Length; i++)
        {
            can_move[i] = true;
        }
    }

    public static int[] GetScores()
    {
        return scores;
    }

    public static bool[] GetCanMove()
    {
        return can_move;
    }

    public static void CheckWinner()
    {
        //Check that no players can make valid moves
        if (!can_move[0] && !can_move[1] && !can_move[2] && !can_move[3])
        {
            int high_score = 0;

            //Check what the highest score achieves is
            for (int i = 0; i < playerCount; i++)
            {
                if (scores[i] > high_score)
                {
                    high_score = scores[i];
                }
            }

            //Any player(s) that got this score are winners
            for (int i = 0; i < playerCount; i++)
            {
                if (scores[i] == high_score)
                {
                    winners.Add(i + 1);
                }
            }

            //Declare the victor and end the game
            if (!GameOver)
            {
                GameData.GameOver = true;
                if(winners.Count == 1)
                {
                    Debug.Log("Game over! Player " + winners[0] + " is the winner with " + high_score + " points!");
                }
                else
                {
                    Debug.Log("Game over! There was a draw! The following players scored " + high_score + "points:");
                    for(int i = 0; i < winners.Count; i++)
                    {
                        Debug.Log(winners[i]);
                    }
                }
            }
        }

        //If the player cant move, speed the AI movements up
        if(!can_move[0] && !speed_increase)
        {
            foreach(GameObject ai in Game.GetCPUs())
            {
                ai.GetComponent<AI>().Delay /= 2;
            }
            speed_increase = true;
        }
        
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

    public static bool GameOver
    {
        get
        {
            return game_over;
        }
        set
        {
            game_over = value;
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

    public static void NextTurn()
    {
        //Make sure the game is running
        if (game_over)
            return;

        //Determine the next player's turn from the previous turn
        switch (playersTurn)
        {
            case 0:
                playersTurn = 1;
                turns++;
                break;
            case 1:
                playersTurn = 2;
                break;
            case 2:
                playersTurn = 3;
                break;
            case 3:
                playersTurn = 4;
                break;
            case 4:
                playersTurn = 1;
                //A full turn is complete after the last player moves
                turns++;
                break;
        }
        Debug.Log("Turn " + Turns + ": player " + PlayersTurn + "'s turn!");
        Game.UpdateTileValues();
    }
}
