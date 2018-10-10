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
	void Start () {
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
        World.RandomiseTiles();

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

                //lambda fun!!
                //Register wants a a function(Tile), but
                // OntileTypeChanged is a function (Tile and GameObject).
                // so we just tell Register to accept it as if it was a function(Tile),
                // and when it is time to run it, run it using the tile feed in in Tile.cs and the tile_go feed in here

                //tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );

                //Use this with a dictonary so we can unregister callbacks in the future
                //Register our callback so that our GameObject gets called whenever the Tile data changes
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


    float randomiseTileTimer = 2f;

	// Update is called once per frame
	void Update () {
        randomiseTileTimer -= Time.deltaTime;

        if(randomiseTileTimer < 0)
        {
            World.RandomiseTiles();
            randomiseTileTimer = 2f;
        }
		
	}

    //THIS IS AN EXAMPLE - NOT IN USE
    void DestroyAllTileGameObjects() {
        //this function might get called when we are changing floors/levels.
        //we need to destroy all visual **GameObjects** -- but not the actual tile data!

        
        while (tileGameObjectMap.Count > 0) {   //keep going until the dictonary is empty
            Tile tile_data = tileGameObjectMap.Keys.First();    //"using system.linq"   to get the First method for dictonary. Dictonaries are inherently unordered, but this lets us get
            GameObject tile_go = tileGameObjectMap[tile_data];      //an arbitray key out, and we don't care about order and are removing them as we go so this works

            //remove the pair from the map
            tileGameObjectMap.Remove(tile_data);

            //unregister the callback!
            tile_data.UnRegisterTileTypeChangedCallback(OnTileTypeChanged);

            //destroy the visual GameObject
            Destroy(tile_go);
        }
        //presumably, after this function gets called, we'd be calling another function
        //to build all the gameobjects for the next level etc
    }


}
