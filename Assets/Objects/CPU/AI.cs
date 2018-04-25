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
    int best_weighting = 0;

    [SerializeField]
    int score;

    [SerializeField]
    List<GameObject> owned_tiles;

    public bool CanMove { get; set; }

    // Use this for initialization
    void Start () {
        score = 0;
        Delay = 1;
        CanMove = true;
        owned_tiles = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if (GameData.GameOver || GameData.PlayersTurn != playerID)
            return;

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
        StartTurn();
        if(OccupyTile())
        {
            EndTurn();
        }
        
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

    bool OccupyTile()
    {
        elapsed += Time.deltaTime;
       
        if (elapsed >= Delay)
        {
            if(selectedTile == null)
            {
                float distance = 0;

                foreach (GameObject tile in owned_tiles)
                {
                    if (tile.GetComponent<TileData>().StackSize > 1)
                    {
                        foreach (GameObject objective_tile in tile.GetComponent<TileData>().GetAccessibleTiles())
                        {
                            distance = Vector3.Distance(objective_tile.gameObject.transform.position, tile.gameObject.transform.position);
                            int temp_weighting = (int)distance + objective_tile.GetComponent<TileData>().TileValue + objective_tile.GetComponent<TileData>().GetSurroundingEmptyTiles();

                            if (temp_weighting > best_weighting)
                            {
                                best_weighting = temp_weighting;
                                occupiedTile = objective_tile;
                                selectedTile = tile;
                            }
                        }
                    }
                }
                selectedTile.GetComponent<TileData>().SelectTile();
                occupiedTile.GetComponent<TileData>().SelectTile();
            }
        }
        else
        {
            return false;
        }

        if (elapsed >= Delay * 2)
        {
            elapsed = 0;
            if (occupiedTile == null)
            {
                UpdateTiles();
                return false;
            }
            else
            {
                Debug.Log(best_weighting);
                int token_min = best_weighting / 2 > selectedTile.GetComponent<TileData>().StackSize - 1 ? selectedTile.GetComponent<TileData>().StackSize - 1 : best_weighting / 2;
                int tokenQuantity = Random.Range(token_min, selectedTile.GetComponent<TileData>().StackSize - 1);
                Debug.Log("Player " + playerID + " has chosen to move " + tokenQuantity + " tokens from TileID: " +
                   selectedTile.GetComponent<TileData>().ID + "(" + selectedTile.GetComponent<TileData>().StackSize + " tokens) to TileID: "
                   + occupiedTile.GetComponent<TileData>().ID + ".");
                selectedTile.GetComponent<TileData>().StackSize -= tokenQuantity;
                Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().ChangeOwnership(playerID);
                Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().StackSize = tokenQuantity;
                Game.GetBoard()[occupiedTile.GetComponent<TileData>().ID].GetComponent<TileData>().UpdateOwnership();
                selectedTile.GetComponent<TileData>().DeselectTile();
                occupiedTile.GetComponent<TileData>().DeselectTile();
                selectedTile = null;
                occupiedTile = null;
                best_weighting = 0;
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
        if (!CanMove)
        {
            CalculateScore();
            EndTurn();
        }
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
            GameData.GetScores()[playerID - 1] = score;
            GameData.GetCanMove()[playerID - 1] = CanMove;
            GameData.NextTurn();
        }
        return;
    }

    void UpdateTiles()
    {
        int accessible_tiles = 0;

        foreach (GameObject tile in owned_tiles)
        {
            tile.GetComponent<TileData>().UpdateAccessibleTiles();
            if (tile.GetComponent<TileData>().StackSize > 1)
                accessible_tiles += tile.GetComponent<TileData>().GetAccessibleTiles().Count;
        }

        if (CanMove && accessible_tiles <= 0)
        {
            CanMove = false;
            Debug.Log("Player " + playerID + "Cannot move.");
            EndTurn();
        }
    }
}
