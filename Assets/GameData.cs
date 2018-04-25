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

    //Use this for initialization
    public static void init()
    {
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
        if (!can_move[0] && !can_move[1] && !can_move[2] && !can_move[3])
        {
            int high_score = 0;
            List<int> winners = new List<int>();

            for (int i = 0; i < playerCount; i++)
            {
                if (scores[i] > high_score)
                {
                    high_score = scores[i];
                }
            }

            for (int i = 0; i < playerCount; i++)
            {
                if (scores[i] == high_score)
                {
                    winners.Add(i + 1);
                }
            }

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
        if(!can_move[0])
        {
            foreach(GameObject ai in Game.GetCPUs())
            {
                ai.GetComponent<AI>().Delay /= 2;
            }
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
        if (game_over)
            return;

        switch (playersTurn)
        {
            case 0:
                playersTurn = 1;
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
                break;
        }
        turns++;
        Debug.Log("Turn " + Turns + ": player " + PlayersTurn + "'s turn!");
        Game.UpdateTileValues();
    }
}
