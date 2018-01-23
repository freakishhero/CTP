using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileData : MonoBehaviour {

    [SerializeField]
    int stack_size;

    [SerializeField]
    byte playerOwner = 0;

    [SerializeField]
    ushort index = 0;

    [SerializeField]
    bool blank = false;

    [SerializeField]
    Sprite[] tileSprites;    

    private GameObject checker;


    private GameObject stack_text;
    

	// Use this for initialization
	void Start () {
        stack_text = this.transform.GetChild(0).gameObject;
        checker = this.transform.GetChild(1).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        updateOwnership();
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

    public byte Owner
    {
        get
        {
            return playerOwner;
        }
        set
        {
            playerOwner = value;
        }
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

    public int getSurroundingEmptyTiles()
    {
        Collider[] tiles = Physics.OverlapSphere(this.transform.position, 2.5f);

        int emptyTiles = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].gameObject.tag == "Tile")
            {
                emptyTiles++;
            }
        }
        return emptyTiles;
    }

    public void setOwnership(byte playerID)
    {
        playerOwner = playerID;
        stack_size = 16;
    }
    public void updateOwnership()
    {
        if(playerOwner > 4)
        {
            playerOwner = 0;
            Debug.Log("PLAYER OWNER CAN'T EXCEED 4.");
        }

        this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[playerOwner];

        if (blank)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = null;


        if (stack_size > 0 && playerOwner > 0)
        {
            gameObject.tag = "Sheep" + playerOwner;
            stack_text.SetActive(true);
            stack_text.GetComponent<TextMesh>().text = "" + stack_size;
        }
        else if (playerOwner == 0 && blank == false)
        {
            gameObject.tag = "Tile";
            stack_text.SetActive(false);
        } else
        {
            gameObject.tag = "Untagged";
            stack_text.SetActive(false);
        }
    }
}
