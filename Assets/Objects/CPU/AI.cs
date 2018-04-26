using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    float elapsed = 0;

    public float Delay { get; set; }

    [SerializeField]
    byte playerID;

    GameObject selectedTile = null;

    GameObject occupiedTile = null;

    int best_weighting = -10000;

    [SerializeField]
    int score;

    [SerializeField]
    List<GameObject> owned_tiles;

    public bool CanMove { get; set; }

    // Use this for initialization
    void Start () {
        score = 0;
        //delay between actions for the AI
        Delay = 0.75f;
        CanMove = true;
        owned_tiles = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        GameData.GetScores()[playerID - 1] = CalculateScore();
        GameData.GetCanMove()[playerID - 1] = CanMove;

        //Ignore if it isnt this AI's turn
        if (GameData.GameOver || GameData.PlayersTurn != playerID)
            return;

        //Take it's turn otherwise
            TakeTurn();
            
	}

    public void SetPlayerID(byte ID)
    {
        playerID = ID;
    }

    public byte GetPlayerID()
    {
        return playerID;
    }

    public int GetScore()
    {
        return score;
    }
    void TakeTurn()
    {
        //Start the AI's turn
        StartTurn();

        //If it has successfully occupied a tile (or is unable to)
        if(OccupyTile())
        {
            //end the AIs turn
            EndTurn();
        }
        
    }

    List<GameObject> GetOwnedTiles()
    {
        owned_tiles.Clear();

        //Checks which tile this particular AI owns
        foreach (GameObject tile in Game.GetBoard())
        {
            if (tile.gameObject.tag == "Sheep" + playerID)
            {
                owned_tiles.Add(tile);
            }
        }
        return owned_tiles;
    } 

    public int CalculateScore()
    {
        score = GetOwnedTiles().Count;
        return score;
    }

    bool OccupyTile()
    {
        //Delta time delay, used to pace the AI's turns
        elapsed += Time.deltaTime;
       
        //If an appropriate delay has passed
        if (elapsed >= Delay)
        {
            //and the AI hasn't selected a tile
            if(selectedTile == null)
            {
                //Loops through all of the tiles owned by the AI
                foreach (GameObject tile in owned_tiles)
                {
                    //Makes sure the AI can move the tile before evaluating it
                    if (tile.GetComponent<TileData>().StackSize > 1)
                    {
                        //Looks at each tile that an evaluated tile can access
                        foreach (GameObject objective_tile in tile.GetComponent<TileData>().GetAccessibleTiles())
                        {
                            //Creates a potential weighting for each tile that the AI can access
                            //through any one of its tiles
                            int temp_weighting = objective_tile.GetComponent<TileData>().TileValue 
                                + objective_tile.GetComponent<TileData>().GetSurroundingEmptyTiles()
                                - tile.GetComponent<TileData>().GetSurroundingEmptyTiles() * 2;

                            //If the evaluated weighting is better than the
                            //last perceive best, the new tile is chosen to be occupied
                            //and its parent is selected
                            if (temp_weighting > best_weighting)
                            {
                                best_weighting = temp_weighting;
                                occupiedTile = objective_tile;
                                selectedTile = tile;
                            }
                        }
                    }
                }
                //Selects the soon-to-be occupied tile and its parent
                selectedTile.GetComponent<TileData>().SelectTile();
                occupiedTile.GetComponent<TileData>().SelectTile();;
            }
        }
        else
        {
            return false;
        }

        //If the previous delay has passed again
        if (elapsed >= Delay * 2)
        {
            //Reset the timer
            elapsed = 0;

            //Ignore if the occupied tile is null
            if (occupiedTile == null)
            {
                UpdateTiles();
                return false;
            }
            //If the tile isn't null
            else
            {
                //Create a temp number of tokens to move
                int temp_token_quantity = selectedTile.GetComponent<TileData>().StackSize / 2
                + (occupiedTile.GetComponent<TileData>().GetSurroundingEmptyTiles() - selectedTile.GetComponent<TileData>().GetSurroundingEmptyTiles());

                //Create a minimum number of tokens to move using the tmep
                int token_min = temp_token_quantity > selectedTile.GetComponent<TileData>().StackSize - 1 ? selectedTile.GetComponent<TileData>().StackSize - 1 : temp_token_quantity;

                //Select a number of tokens to move using the weighted minimum
                int token_quantity = Random.Range(token_min, selectedTile.GetComponent<TileData>().StackSize - 1);

                //If there is only one space around the tile, override to move all tokens out to avoid being trapped.
                if (selectedTile.GetComponent<TileData>().GetSurroundingEmptyTiles() == 1)
                    token_quantity = selectedTile.GetComponent<TileData>().StackSize - 1;

                Debug.Log("Player " + playerID + " has chosen to move " + token_quantity + " tokens from TileID: " +
                   selectedTile.GetComponent<TileData>().ID + "(" + selectedTile.GetComponent<TileData>().StackSize + " tokens) to TileID: "
                   + occupiedTile.GetComponent<TileData>().ID + ".");

                //The selected tile loses tokens equal to the number choesn by the AI
                selectedTile.GetComponent<TileData>().StackSize -= token_quantity;

                //Update the newly occupied tile for ownership to the AI and stack size according to the moved token count
                Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().ChangeOwnership(playerID);
                Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().StackSize = token_quantity;
                Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().UpdateOwnership();

                //Deselect the tiles involved
                selectedTile.GetComponent<TileData>().DeselectTile();
                occupiedTile.GetComponent<TileData>().DeselectTile();

                selectedTile = null;
                occupiedTile = null;

                //Reset the weighting comparison value for next time
                best_weighting = -10000;
                return true;
            }
        }
        else
        {
            return false;
        }

        
    }

    void StartTurn()
    {
        //If the AI cant move immediately end its turn
        if (!CanMove)
        {
            CalculateScore();
            EndTurn();
        }
        //Start the turn if the AI can move
        else
        {
            CalculateScore();
            UpdateTiles();
        }
        
    }

    void EndTurn()
    {
        if(playerID == GameData.PlayersTurn)
        {
            //Process the next turn
            GameData.NextTurn();
        }
        return;
    }

    void UpdateTiles()
    {
        int accessible_tiles = 0;

        //Update the accessible tiles for each of the AI's owned tiles
        foreach (GameObject tile in owned_tiles)
        {
            tile.GetComponent<TileData>().UpdateAccessibleTiles();
            if (tile.GetComponent<TileData>().StackSize > 1)
                accessible_tiles += tile.GetComponent<TileData>().GetAccessibleTiles().Count;
        }

        //If the AI has no accessible tiles it can no longer move.
        if (CanMove && accessible_tiles <= 0)
        {
            CanMove = false;
            Debug.Log("Player " + playerID + "Cannot move.");
            EndTurn();
        }
    }
}
