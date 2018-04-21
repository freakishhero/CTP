using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("It is currently player " + GameData.PlayersTurn + "'s turn.");
        }

        if (GameData.PlayersTurn == 1)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SelectTile();
            }
        }
    }

    void EndTurn()
    {
        if (GameData.PlayersTurn < 4)
        {
            if (GameData.PlayersTurn > 0 && GameData.PlayersTurn < 4)
                Game.getCPUs()[GameData.PlayersTurn - 1].GetComponent<AI>().setTurnState(false);

            GameData.PlayersTurn++;
        }
        else
        {
            GameData.PlayersTurn = 1;
        }
        Debug.Log("Player " + GameData.PlayersTurn + "'s turn.");
    }

    bool SelectTile()
    {
        Vector2 target = new Vector2
            (
            this.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x,
            this.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).y
            );

        RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero, 0f);
        //Debug.DrawLine(transform.position, target);
        //Debug.Log(target);

        if (hit.collider == null)
            return false;

        if (hit.collider.gameObject.tag != "Sheep1")
            return false;

        TileData data = hit.collider.gameObject.GetComponent<TileData>();

        //LOGIC FOR SPLITTING STACK
        if (data.StackSize > 1)
        {
            bool moved = false;
            ushort lastIDqueried = 0;
            int queriedTiles = 0;
            foreach (GameObject tile in Game.getBoard())
            {
                if (!moved)
                {
                    if (tile != hit.collider.gameObject)
                    {
                        TileData d = tile.GetComponent<TileData>();
                        Debug.Log("Tile position: " + tile.transform.position);
                        Debug.Log("checker position: " + data.TileChecker.transform.position);
                        if (data.TileChecker.transform.position == tile.transform.position && tile.gameObject.tag == "Tile")
                        {
                            queriedTiles++;
                            lastIDqueried = d.ID;
                            Debug.Log("On an empty tile. Tile ID is:" + lastIDqueried);
                        }
                        else
                        {
                            moved = true;
                            if (queriedTiles > 0)
                            {
                                data.StackSize /= 2;
                                Game.getBoard()[lastIDqueried].GetComponent<TileData>().Owner = 1;
                                Game.getBoard()[lastIDqueried].GetComponent<TileData>().StackSize = data.StackSize;
                                Debug.Log("Hit an occupied tile or the end of the game board. Cannot progress. Last unoccupied tile was " + lastIDqueried);
                            }
                            else
                            {
                                Debug.Log("This stack cannot move in that direction.");
                            }

                        }
                    }
                    data.TileChecker.transform.position += Vector3.right * 2;
                }
            }
            moved = false;
            queriedTiles = 0;
            data.TileChecker.transform.position = hit.collider.transform.position;
            lastIDqueried = 0;

            return true;
        }
        return false;
    }

}
