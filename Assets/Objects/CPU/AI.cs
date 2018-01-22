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

    // Use this for initialization
    void Start () {
        ownedTiles = new List<GameObject>();
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
            NextTurn();
        }
    }

    List<GameObject> getOwnedTiles()
    {
        ownedTiles.Clear();
        foreach (GameObject tile in Game.getBoard())
        {
            if (tile.gameObject.tag == "Sheep" + playerID)
            {
                ownedTiles.Add(tile);
            }
        }
        return ownedTiles;
    } 

    int calculateScore()
    {
        int total = 0;
        foreach(GameObject tile in getOwnedTiles())
        {
            total++;
        }
        return total;
    }

    GameObject SelectTile()
    {
        GameObject chosenTile = null;

        foreach (GameObject tile in getOwnedTiles())
        {
            if(chosenTile == null)
            {
                chosenTile = tile;
            }
            else if (tile.GetComponent<TileData>().StackSize > chosenTile.GetComponent<TileData>().StackSize)
            {
                chosenTile = tile;
            }
        }

        if (chosenTile != null)
        {
            Debug.Log("Player " + playerID + " has chosen to move the stack at TileID(" + chosenTile.GetComponent<TileData>().ID + ")");
        }
        else
        {
            Debug.Log("There are no tiles to select for player " + playerID);
        }
        return chosenTile;
    }

    bool OccupyTile()
    {
        GameObject selectedTile = SelectTile();
        GameObject occupiedTile = null;
        float minDistance = 100000;

        foreach (GameObject tile in Game.getBoard())
        {
            if (tile.tag == "Tile")
            {
                if(Vector3.Distance(selectedTile.gameObject.transform.position, tile.gameObject.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(selectedTile.gameObject.transform.position, tile.gameObject.transform.position);
                    occupiedTile = tile;
                }
            }
        }
        if (occupiedTile != null && selectedTile.GetComponent<TileData>().StackSize > 1)
        {
            selectedTile.GetComponent<TileData>().StackSize /= 2;
            Game.getBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().Owner = playerID;
            Game.getBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().StackSize = selectedTile.GetComponent<TileData>().StackSize;
            Debug.Log("Player " + playerID + " has chosen to move the selected stack to TileID(" + occupiedTile.GetComponent<TileData>().ID + ")");
            return true;
        }
        else
        {
            Debug.Log("Player " + playerID + " has no tiles with a stacksize greater than one.");
        }
        Debug.Log("There are no tiles to occupy");
        return false;
    }

    void StartTurn()
    {
        score = calculateScore();
        takenTurn = true;
    }
    void NextTurn()
    {
    }
}
