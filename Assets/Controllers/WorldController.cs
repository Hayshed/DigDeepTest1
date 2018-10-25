using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



public class WorldController : MonoBehaviour {

    //going to make an instance of WorldController, for other classes to "get" from
    public static WorldController Instance { get; protected set; }

    Dictionary<Tile, GameObject> tileGameObjectMap;

    

    public Sprite airSprite;
    public Sprite dirtSprite;

    public World World { get; protected set; }


	// Use this for initialization
	void OnEnable () {
        if (Instance != null)
        {
            Debug.LogError("There should never be two world controllers");
        }
        Instance = this;

        //make a new world passing no variables, leaving as default constructor settings
        World = new World();

        //instantiate our dictonary that tracks which GameObejct is rendering which Tile data
        tileGameObjectMap = new Dictionary<Tile, GameObject>();


        

        





        //randomise all the tiles in the world
        //World.RandomiseTiles();

        World.InitialiseTiles();

        //create a gameObject for each of our tiles
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Tile tile_data = World.GetTileAt(x, y);

                //create new GameObject with unique name
                GameObject tile_go = new GameObject();

                //Add our tile/GO pair to the dictonary
                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;

                //set intial position of each tile GameObject
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);

                tile_go.transform.SetParent(this.transform, true);

                // Add a sprite renderer to each tile GameObject
                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();

                
                tile_data.RegisterTileTypeChangedCallback( OnTileTypeChanged);

                //set the sprite depending on what kind of tile it is

                if (tile_data.Type == TileType.Air)
                {
                    tile_sr.sprite = airSprite;
                }
                else if (tile_data.Type == TileType.Dirt)
                {
                    tile_sr.sprite = dirtSprite;
                }

                //run once to intialise all tile game Objects with their first visual
                OnTileTypeChanged(tile_data);


                




            }

        }

        Debug.Log("WorldControler initallse");
        World.RegisterTileChanged(OnTileTypeChanged);

    }


    

    //should be called whenever the type of a tile changes in the data,
    // so that the visual gameobjects can change to match
    void OnTileTypeChanged(Tile tile_data)
    {
        if(tileGameObjectMap.ContainsKey(tile_data) == false) {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you add the tile to the dictonary? Or not unregesiter a callback?");
            return;
        }
        GameObject tile_go = tileGameObjectMap[tile_data];
        if (tile_go == null) {
            Debug.LogError("tileGameObjectMap returned GameObject is null -- did you add the tile to the dictonary? Or not unregesiter a callback?");
            return;
        }

        if(tile_data.Type == TileType.Air)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = airSprite;
        }
        else if (tile_data.Type == TileType.Dirt)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = dirtSprite;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged -- Don't have a sprite for that tile type");
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }

    }


   // Update runs every frame, activating update functions downstream and telling them how much time passed between frames
	void Update () {

        
        World.Update(Time.deltaTime);
		
	}

  


}
