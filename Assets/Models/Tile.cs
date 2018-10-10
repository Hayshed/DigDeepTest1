using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//declare an enum to store the different types of tiles
//by having this outside the class and public, Tiletype can be called instead of tile.Tiletype.
//This is the kind of global shared information we should do this with
public enum TileType { Air, Dirt, Stone, Ore }


public class Tile {


    Action<Tile> cbTileTypeChanged; 


    // // What is this class storing?
   

    //store the type of tile, by default be Air
    TileType type = TileType.Air;

    public TileType Type
    {
        get
        {
            return type;
        }
        set
        {
            TileType oldType = type;
            type = value;
            //call the callback, let the world controller know we have changed
            if (cbTileTypeChanged != null && oldType != type)
            {
                cbTileTypeChanged(this);
            }
            
        }
    }

    //store it's location in the world grid
    private int x;
    private int y;

    public int X
    {
        get { return x; }
    }

    public int Y
    {
        get
        {
            return y;
        }

        
    }



    //store the world it belongs to
    //will be more important later as different grids are used
    World world;


    




    //Constructor, called when made
    //needs to know the world it's in, and it's tile coords
    public Tile(World world, int xTmp, int yTmp)
    {
        // using "this" to avoid confustion between local and global variables within this class
        this.world = world;
        this.x = xTmp;
        this.y = yTmp;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback; 
    }

    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }



}
