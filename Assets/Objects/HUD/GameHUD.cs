using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour {

    [SerializeField]
    Text[] UIText;

    [SerializeField]
    Text victory_text;

	// Use this for initialization
	void Start () {
        //disable victory text
        victory_text.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        //Update player scores
        for (int i = 2; i < UIText.Length; i++)
        {
            UIText[i].text = "Player " + (i - 1) + ": " + GameData.GetScores()[i - 2];
        }

        //Update the players turn
        UIText[0].text = "Player " + GameData.PlayersTurn + "'s turn! (Turn " + GameData.Turns + ")";


        //Display victory message
        if (GameData.GameOver)
        {
            //enable victory text
            victory_text.gameObject.SetActive(true);

            //Is player one the only winner?
            if (GameData.winners.Contains(1) && GameData.winners.Count == 1)
            {
                victory_text.text = "Congratulations! You win!";
            }
            //is player one one of the winners?
            else if (GameData.winners.Contains(1) && GameData.winners.Count > 1)
            {
                victory_text.text = "It's a draw!";
            }
            //player one lost
            else
            {
                victory_text.text = "You lose...";
            }
        }
            
    }
}
