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
    List<GameObject> owned_tiles;

    [SerializeField]
    List<int> tile_weightings;

    // Use this for initialization
    void Start () {
        score = 0;
        owned_tiles = new List<GameObject>();
        tile_weightings = new List<int>();
    }
	
	// Update is called once per frame
	void Update () {
        if (GameData.PlayersTurn == playerID && takenTurn == false)
        {
            TakeTurn();
        }
        else
        {
        }
	}

    public void SetPlayerID(byte ID)
    {
        playerID = ID;
    }

    public void SetTurnState(bool state)
    {
        takenTurn = state;
    }

    public byte GetPlayerID()
    {
        return playerID;
    }

    void TakeTurn()
    {
        StartTurn();
        if(OccupyTile())
        {
        }
        EndTurn();
    }

    List<GameObject> GetOwnedTiles()
    {
        owned_tiles.Clear();
        foreach (GameObject tile in Game.GetBoard())
        {
            if (tile.gameObject.tag == "Sheep" + playerID)
            {
                owned_tiles.Add(tile);
            }
        }
        return owned_tiles;
    } 

    void CalculateScore()
    {
        score = GetOwnedTiles().Count;
    }

    GameObject SelectTile()
    {
        GameObject chosenTile = null;
        tile_weightings.Clear();
        int weighting = 0;

        foreach (GameObject tile in GetOwnedTiles())
        {
            if (tile.GetComponent<TileData>().StackSize > 1)
            {
                weighting += tile.GetComponent<TileData>().StackSize;

                if (tile.GetComponent<TileData>().GetSurroundingEmptyTiles() >= 1)
                {
                    weighting += tile.GetComponent<TileData>().GetSurroundingEmptyTiles();
                }
            }

            tile_weightings.Add(weighting);
            weighting = 0;
        }

        for (int i = 0; i < tile_weightings.Count; i++)
        {
            
            if(tile_weightings[i] > weighting)
            {
                weighting = tile_weightings[i];
                if(owned_tiles[i].GetComponent<TileData>().StackSize > 1)
                chosenTile = owned_tiles[i];
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
        takenTurn = true;
    }

    void EndTurn()
    {
        CalculateScore();
        UpdateTiles();
        GameData.NextTurn();
    }

    void UpdateTiles()
    {
        foreach (GameObject tile in owned_tiles)
        {
            tile.GetComponent<TileData>().UpdateAccessibleTiles();
        }
    }
}
