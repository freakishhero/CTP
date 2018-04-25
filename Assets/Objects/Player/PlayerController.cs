using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum PlayerState
    {
        IDLE = 0,
        TILE_CHOSEN = 1,
        DIRECTION_CHOSEN = 2
    }

    PlayerState state = PlayerState.IDLE;

    [SerializeField]
    int score;

    bool taken_turn = false;

    [SerializeField]
    List<GameObject> owned_tiles;
    TileData chosen_tile_data;
    public bool CanMove { get; set; }

    // Use this for initialization
    void Start()
    {
        CanMove = true;
        owned_tiles = new List<GameObject>();
        score = 0;
    }

    void Update()
    {
        if (GameData.GameOver || GameData.PlayersTurn != 1)
            return;

        if(!taken_turn)
        {
            taken_turn = true;
            CalculateScore();
            UpdateTiles();
        }

        if (Input.GetKeyDown("space"))
        {
            CalculateScore();
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("updating tiles");
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (GameData.PlayersTurn == 1)
            {
                HandleClick();
            }
            else
            {
                Debug.Log("It's not your turn!");
            }
        }
    }

    bool HandleClick()
    {
        Vector3 target = new Vector3
            (
            this.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x,
            this.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).y, 0
            );

        RaycastHit hit;
        Physics.Raycast(target, transform.TransformDirection(Vector3.forward), out hit, 100f);

        if (hit.collider == null)
            return false;

        TileData data = hit.collider.gameObject.GetComponent<TileData>();

        if (state == PlayerState.IDLE)
        {
            if (hit.collider.gameObject.tag != "Sheep1")
                return false;

            if (data.GetAccessibleTiles().Count < 1 || data.StackSize < 2)
            {
                //Debug.Log("This tile cannot move");
                return false;
            }

            chosen_tile_data = data;
            chosen_tile_data.stack_text.GetComponent<TextMesh>().fontSize = 60;
            chosen_tile_data.TempStackSize = chosen_tile_data.StackSize - 1;

            Collider[] collision;

            for (int i = 0; i < Directions.directions.Count; i++)
            {
                collision = Physics.OverlapSphere(data.transform.position + Directions.directions[i], 0.5f);

                if (collision.Length == 1)
                {
                    if (collision[0].gameObject.tag == "Tile")
                    {
                        collision[0].gameObject.tag = "Arrow";
                        collision[0].gameObject.transform.Rotate(new Vector3(0, 0, -60 * i));
                        collision[0].GetComponent<TileData>().Direction = (byte)i;
                    }
                }
                collision = null;
            }
            state = PlayerState.TILE_CHOSEN;
            return false;
        }

        if(state == PlayerState.TILE_CHOSEN)
        {   
            if(hit.collider.gameObject.tag == "Arrow")
            {
                state = PlayerState.DIRECTION_CHOSEN;

                bool furthest_found = false;
                int last_checked_ID = 0;
                chosen_tile_data.TileChecker.transform.position = chosen_tile_data.transform.position;
                Collider[] checked_tile = null;
                while (!furthest_found)
                {
                    chosen_tile_data.TileChecker.transform.position += Directions.directions[data.Direction];
                    checked_tile = Physics.OverlapSphere(chosen_tile_data.TileChecker.transform.position, 0.5f);

                    if (checked_tile.Length == 1)
                    {
                        if (checked_tile[0].gameObject.tag == "Tile" || checked_tile[0].gameObject.tag == "Arrow")
                            last_checked_ID = checked_tile[0].GetComponent<TileData>().ID;
                        else
                            furthest_found = true;
                    }
                    else
                    {
                        furthest_found = true;
                    }
                }

                if (last_checked_ID > 0)
                {
                    chosen_tile_data.StackSize -= chosen_tile_data.TempStackSize;
                    Game.GetBoard()[last_checked_ID].GetComponent<TileData>().ChangeOwnership(1);
                    Game.GetBoard()[last_checked_ID].GetComponent<TileData>().StackSize = chosen_tile_data.TempStackSize;
                    state = PlayerState.IDLE;
                    chosen_tile_data.stack_text.GetComponent<TextMesh>().fontSize = 30;
                    chosen_tile_data.TempStackSize = 0;
                    EndTurn();
                    foreach (GameObject tile in Game.GetBoard())
                    {
                            tile.GetComponent<TileData>().DestroyArrow();
                    }
                    return true;
                }
            }
            else if (hit.collider.gameObject.GetComponent<TileData>() == chosen_tile_data)
            {
                chosen_tile_data.TempStackSize--;
                if (chosen_tile_data.TempStackSize < 1)
                    chosen_tile_data.TempStackSize = chosen_tile_data.StackSize - 1;
            }
            else
            {
                foreach (GameObject tile in Game.GetBoard())
                {
                        tile.GetComponent<TileData>().DestroyArrow();
                }
                state = PlayerState.IDLE;
                chosen_tile_data.stack_text.GetComponent<TextMesh>().fontSize = 30;
                chosen_tile_data.TempStackSize = 0;
            }
        }
        return false;
    }

    void CalculateScore()
    {
        score = GetOwnedTiles().Count;
    }

    void EndTurn()
    {
        GameData.GetScores()[0] = score;
        GameData.GetCanMove()[0] = CanMove;
        taken_turn = false;
        GameData.NextTurn();
        return;
    }
    
    void UpdateTiles()
    {
        int accessible_tiles = 0;

        foreach (GameObject tile in owned_tiles)
        {
            tile.GetComponent<TileData>().UpdateAccessibleTiles();

            if(tile.GetComponent<TileData>().StackSize > 1)
                accessible_tiles += tile.GetComponent<TileData>().GetAccessibleTiles().Count;
        }

        if (accessible_tiles <= 0)
        {
            CanMove = false;
            EndTurn();
        }   
    }

    List<GameObject> GetOwnedTiles()
    {
        owned_tiles.Clear();
        foreach (GameObject tile in Game.GetBoard())
        {
            if (tile.gameObject.tag == "Sheep1")
            {
                owned_tiles.Add(tile);
            }
        }
        return owned_tiles;
    }
}
