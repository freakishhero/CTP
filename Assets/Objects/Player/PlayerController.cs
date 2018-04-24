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
            GameData.EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("It is currently player " + GameData.PlayersTurn + "'s turn.");
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (GameData.PlayersTurn == 1)
            {
                SelectTile();
            }
            else
            {
                Debug.Log("It's not your turn!");
            }
        }
    }

    bool SelectTile()
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

        if (hit.collider.gameObject.tag != "Sheep1")
            return false;

        TileData data = hit.collider.gameObject.GetComponent<TileData>();

        if (data.GetSurroundingEmptyTiles() < 1)
        {
            Debug.Log("This tile cannot move");
            return false;
        }
        
        //LOGIC FOR SPLITTING STACK
        if (data.StackSize > 1)
        {
            bool moved = false;
            ushort lastIDqueried = 0;
            int queriedTiles = 0;
            foreach (GameObject tile in Game.GetBoard())
            {
                if (!moved)
                {
                    if (tile != hit.collider.gameObject)
                    {
                        if (data.TileChecker.transform.position == tile.transform.position && tile.gameObject.tag == "Tile")
                        {
                            queriedTiles++;
                            lastIDqueried = tile.GetComponent<TileData>().ID;
                        }
                        else
                        {
                            moved = true;
                            if (queriedTiles > 0)
                            {
                                data.StackSize /= 2;
                                Game.GetBoard()[lastIDqueried].GetComponent<TileData>().Owner = 1;
                                Game.GetBoard()[lastIDqueried].GetComponent<TileData>().StackSize = data.StackSize;
                                GameData.EndTurn();
                            }
                            else
                            {
                                Debug.Log("This stack cannot move in that direction.");
                            }
                        }
                    }
                    data.TileChecker.transform.position += Directions.east;
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
