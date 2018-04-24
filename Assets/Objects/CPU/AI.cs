using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    [SerializeField]
    byte playerID;

    [SerializeField]
    bool takenTurn;

    [SerializeField]
    int score;

    [SerializeField]
    List<GameObject> ownedTiles;

    [SerializeField]
    List<int> tileWeightings;

    // Use this for initialization
    void Start () {
        ownedTiles = new List<GameObject>();
        tileWeightings = new List<int>();
    }
	
	// Update is called once per frame
	void Update () {
        if (GameData.PlayersTurn == playerID && takenTurn == false)
        {
            takeTurn();
        }
        else
        {
        }
	}

    public void setPlayerID(byte ID)
    {
        playerID = ID;
    }

    public void setTurnState(bool state)
    {
        takenTurn = state;
    }

    public byte getPlayerID()
    {
        return playerID;
    }

    void takeTurn()
    {
        StartTurn();
        if(OccupyTile())
        {
        }
        GameData.EndTurn();
    }

    List<GameObject> getOwnedTiles()
    {
        ownedTiles.Clear();
        foreach (GameObject tile in Game.GetBoard())
        {
            if (tile.gameObject.tag == "Sheep" + playerID)
            {
                ownedTiles.Add(tile);
            }
        }
        return ownedTiles;
    } 

    void calculateScore()
    {
        score = getOwnedTiles().Count;
    }

    GameObject SelectTile()
    {
        GameObject chosenTile = null;
        tileWeightings.Clear();
        int tileWeighting = 0;

        foreach (GameObject tile in getOwnedTiles())
        {
            if (tile.GetComponent<TileData>().StackSize > 1)
            {
                tileWeighting += tile.GetComponent<TileData>().StackSize;

                if (tile.GetComponent<TileData>().GetSurroundingEmptyTiles() >= 1)
                {
                    tileWeighting += tile.GetComponent<TileData>().GetSurroundingEmptyTiles();
                }
            }

            tileWeightings.Add(tileWeighting);
            tileWeighting = 0;
        }

        for (int i = 0; i < tileWeightings.Count; i++)
        {
            
            if(tileWeightings[i] > tileWeighting)
            {
                tileWeighting = tileWeightings[i];
                if(ownedTiles[i].GetComponent<TileData>().StackSize > 1)
                chosenTile = ownedTiles[i];
            }
        }

        if (chosenTile == null)
        {
            Debug.Log("Player " + playerID + " has no tiles that they can move.");
        }

        return chosenTile;
    }

    bool OccupyTile()
    {
        GameObject selectedTile = SelectTile();
        GameObject occupiedTile = null;
        float distance = 0;
        int best_weighting = 0;

        if (selectedTile == null)
        {
            return false;
        }

        foreach (GameObject tile in Game.GetBoard())
        {
            if (tile.tag == "Tile")
            {
                if (Vector3.Distance(selectedTile.gameObject.transform.position, tile.gameObject.transform.position) > distance)
                {
                    distance = Vector3.Distance(selectedTile.gameObject.transform.position, tile.gameObject.transform.position);
                    best_weighting = (int)distance + tile.GetComponent<TileData>().TileValue;
                    occupiedTile = tile;
                }
            }
        }

        if (occupiedTile == null)
        {
            Debug.Log("There are no tiles to occupy.");
            return false;
        }
        else
        {
            int tokenQuantity = Random.Range(1, selectedTile.GetComponent<TileData>().StackSize - 1);
            Debug.Log("Player " + playerID + " has chosen to move " + tokenQuantity + " tokens from TileID: " + 
                selectedTile.GetComponent<TileData>().ID + "(" + selectedTile.GetComponent<TileData>().StackSize + " tokens) to TileID: " 
                + occupiedTile.GetComponent<TileData>().ID + ".");
            selectedTile.GetComponent<TileData>().StackSize -= tokenQuantity;
            Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().Owner = playerID;
            Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().StackSize = tokenQuantity;
            Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().UpdateOwnership();
            Debug.Log(best_weighting);
            return true;
        }
    }

    void StartTurn()
    {
        calculateScore();
        takenTurn = true;
    }
}
