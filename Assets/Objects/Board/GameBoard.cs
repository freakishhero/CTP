using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    [SerializeField]
    GameObject tile;

    [SerializeField]
    GameObject emptytile;

    [SerializeField]
    List<GameObject> board;

    Vector3 location = new Vector3(0,0,0);
    // Use this for initialization
    void Start () {
        byte iterator = 0; //0 to 256
        ushort spawn_amount = 225; //0 and 65535
        sbyte x_location = 0; //-128 to 127    
        float y_distance = -1.75f;
        byte x_distance = 2;

        for(int i = 0; i < spawn_amount; i++)
        {
            GameObject instancedTile = (GameObject)Instantiate(tile, location, Quaternion.identity);
            board.Add(instancedTile);
            location.x += x_distance;
            iterator++;
            if(iterator == 15)
            {
                iterator = 0;
                if (x_location == 0)
                    x_location = -1;
                else
                    x_location = 0;
                location.x = x_location;
                location.y -= y_distance;
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
