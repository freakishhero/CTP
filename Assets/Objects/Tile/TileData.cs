using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileData : MonoBehaviour {

    [SerializeField]
    int stack_size;

    public int TempStackSize { get; set; }

    [SerializeField]
    byte playerOwner = 0;

    [SerializeField]
    ushort index = 0;

    [SerializeField]
    bool blank = false;

    [SerializeField]
    Sprite[] tileSprites;

    [SerializeField]
    int tile_value;

    byte direction = 0;

    public bool Selected { get; set; }

    [SerializeField]
    List<GameObject> accessible_tiles;

    private GameObject checker;

    public GameObject stack_text;

    Quaternion default_rotation;

	// Use this for initialization
	void Start () {
        TempStackSize = 0;
        Selected = false;
        stack_text = this.transform.GetChild(0).gameObject;
        checker = this.transform.GetChild(1).gameObject;
        default_rotation = this.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateOwnership();
    }

    public int StackSize
    {
        get
        {
            return stack_size;
        }
        set
        {
            stack_size = value;
        }
    }

    public byte Direction
    {
        get
        {
            return direction;
        }
        set
        {
            direction = value;
        }
    }

    public byte GetOwner
    {
        get
        {
            return playerOwner;
        }
    }

    public void DestroyArrow()
    {
        direction = 0;
        this.gameObject.tag = "Tile";
        this.transform.rotation = default_rotation;
    }

    public GameObject TileChecker
    {
        get
        {
            return checker;
        }
    }

    public ushort ID
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    public bool RemoveTile
    {
        get
        {
            return blank;
        }
        set
        {
            blank = value;
        }
    }

    public int TileValue
    {
        get
        {
            return tile_value;
        }
        set
        {
            tile_value = value;
        }
    }

    public List<GameObject> GetAccessibleTiles()
    {
        return accessible_tiles;
    }

    public void ChangeOwnership(byte new_owner)
    {
        playerOwner = new_owner;
        this.gameObject.tag = "Sheep" + new_owner;
    }

    public void SelectTile()
    {
        if(!Selected)
        {
            Selected = true;
        }
    }

    public void DeselectTile()
    {
        if (Selected)
        {
            Selected = false;
        }
    }

    public void UpdateOwnership()
    {
        if (playerOwner > 4)
        {
            playerOwner = 0;
            Debug.Log("PLAYER OWNER CAN'T EXCEED 4.");
        }

        if(this.gameObject.tag != "Arrow")
        {
            if (this.playerOwner == 1 && StackSize > 1 && accessible_tiles.Count > 0 && GameData.PlayersTurn == 1)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[7];
                
            }

            else if (this.playerOwner == 0 && Selected)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[11];
            }
            else if (this.playerOwner != 1 && Selected)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[6 + playerOwner];
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[playerOwner];
            }
            
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[6];
            return;
        }

        if (blank)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = null;

        if (stack_size > 0 && playerOwner > 0)
        {
            gameObject.tag = "Sheep" + playerOwner;
            gameObject.name = "Tile_Sheep_" + playerOwner;
            stack_text.SetActive(true);
            if(TempStackSize > 0)
                stack_text.GetComponent<TextMesh>().text = "" + TempStackSize;
            else
                stack_text.GetComponent<TextMesh>().text = "" + stack_size;
        }
        else if (playerOwner == 0 && blank == false)
        {
            gameObject.tag = "Tile";
            gameObject.name = "Tile";
            stack_text.SetActive(false);
        }
        else
        {
            gameObject.tag = "Empty";
            gameObject.name = "Blank_Tile";
            stack_text.SetActive(false);
        }
    }

    public int GetValue()
    {
        int weighting = 0;
        Collider[] collision;

        for (int i = 0; i < Directions.directions.Count; i++)
        {
            collision = Physics.OverlapSphere(this.transform.position + Directions.directions[i], 0.5f);

            if (collision.Length == 1)
            {
                if (collision[0].gameObject.tag == ("Sheep1"))
                {
                    weighting = weighting + (1 + (int)collision[0].GetComponent<TileData>().StackSize / 2) + 1;
                }
            }
            collision = null;
        }
        return weighting;
    }

    public int GetSurroundingEmptyTiles()
    {
        Collider[] tiles = Physics.OverlapSphere(this.transform.position, 2.0f);

        int emptyTiles = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            TileData td = tiles[i].GetComponent<TileData>();
            if (td != this && td.GetOwner == 0 && td.blank == false)
            {
                emptyTiles++;
            }
        }
        return emptyTiles;
    }

    public void UpdateAccessibleTiles()
    {
        accessible_tiles.Clear();

        //Index equal to Direction.Directions
        bool[] movable_direction = new bool[6];
        Collider[] collision;

        for (int i = 0; i < movable_direction.Length; i++)
        {
            collision = Physics.OverlapSphere(this.transform.position + Directions.directions[i], 0.5f);

            if (collision.Length == 1)
            {
                if (collision[0].gameObject.tag == "Tile")
                {
                    movable_direction[i] = true;
                }
                else
                {
                    movable_direction[i] = false;
                }
            }
            collision = null;
        }

        for (int i = 0; i < movable_direction.Length; i++)
        {
            if(movable_direction[i] == true)
            {
                bool furthest_found = false;
                int last_checked_ID = 0;
                checker.transform.position = this.transform.position;
                Collider[] checked_tile = null;
                while (!furthest_found)
                {
                    checker.transform.position += Directions.directions[i];
                    checked_tile = Physics.OverlapSphere(checker.transform.position, 0.5f);

                    if(checked_tile.Length == 1)
                    {
                        if(checked_tile[0].gameObject.tag == "Tile")
                            last_checked_ID = checked_tile[0].GetComponent<TileData>().ID;
                        else
                            furthest_found = true;
                    }
                    else
                    {
                        furthest_found = true;
                    }
                }
                accessible_tiles.Add(Game.GetBoard()[last_checked_ID]);
            }
        }
        //Do a Physics.checkSphere at each of the immediate locations to check which directions you can move.
        //Ray cast in every direction? or use the tile checker
    }

    public void SetOwnership(byte playerID)
    {
        playerOwner = playerID;
        stack_size = 16;
        this.gameObject.tag = "Sheep" + playerID;
    }

    void OnDrawGizmos()
    {
        if(playerOwner > 0 && accessible_tiles.Count > 0)
        {
           // for (int i = 0; i < 6; i++)
           // {
           //     Gizmos.DrawSphere(this.transform.position + Directions.directions[i], 0.5f);
           // }
                
        }
       
    }
}
