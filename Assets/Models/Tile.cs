using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//declare an enum to store the different types of tiles
//by having this outside the class and public, Tiletype can be called instead of tile.Tiletype.
//This is the kind of global shared information we should do this with
public enum TileType { Air, Dirt, Stone, Ore }

public enum BackgroundType { Air, Dirt}


public class Tile {


    Action<Tile> cbTileTypeChanged; 


    // // What is this class storing?
   

    //store the type of tile, by default be Air
    TileType type = TileType.Air;
    BackgroundType backType = BackgroundType.Air;
    private float movementCost;
    public float MovementCost {
        get {
            if (type == TileType.Air) {
                return movementCost;
            }
            else {
                return 0;
            }
        } 
    }

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
    public World world;


    




    //Constructor, called when made
    //needs to know the world it's in, and it's tile coords
    public Tile(World world, int xTmp, int yTmp)
    {
        // using "this" to avoid confustion between local and global variables within this class
        this.world = world;
        this.x = xTmp;
        this.y = yTmp;

        movementCost = 1f;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback; 
    }

    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="diagOkay"> Is diagonal movement ok></param>
    /// <param name="clippingOkay"> Is clipping corners / squeezing diagonally ok?</param>
    /// <returns></returns>
    public Tile[] GetNeighbours(bool diagOkay = false) {

        Tile[] ns;

        if (diagOkay == false) { //Tile order N E S W
            ns = new Tile[4];
        }
        else {
            ns = new Tile[8]; //Tile order N E S W NE SE SW NW
        }

        Tile n;

        n = world.GetTileAt(X, Y + 1);
        ns[0] = n;  // could be null, but thats ok
        n = world.GetTileAt(X + 1, Y);
        ns[1] = n;
        n = world.GetTileAt(X, Y - 1);
        ns[2] = n;
        n = world.GetTileAt(X - 1, Y);
        ns[3] = n;

        if (diagOkay == true) {

            n = world.GetTileAt(X + 1, Y + 1);
            ns[4] = n;  // could be null, but thats ok
            n = world.GetTileAt(X + 1, Y - 1);
            ns[5] = n;
            n = world.GetTileAt(X - 1, Y - 1);
            ns[6] = n;
            n = world.GetTileAt(X - 1, Y + 1);
            ns[7] = n;

        }

        return ns;

    }

    // Returns true if there is at least one block to stand on in the 3 blocks below
    public bool IsStandable() {

        if (world.GetTileAt(X - 1, Y - 1) != null){                         // First check if the tile is not out of bounds
            if (world.GetTileAt(X - 1, Y - 1).Type != TileType.Air) {       // Second check if the tile is not air
                return true;                                                // If the tile is in bounds and not air, it's standable
            }
        }
        if (world.GetTileAt(X, Y - 1) != null) {                            // First check if the tile is not out of bounds
            if (world.GetTileAt(X, Y - 1).Type != TileType.Air) {           // Second check if the tile is not air
                return true;                                                // If the tile is in bounds and not air, it's standable
            }
        }
        if (world.GetTileAt(X + 1, Y - 1) != null) {                        // First check if the tile is not out of bounds
            if (world.GetTileAt(X + 1, Y - 1).Type != TileType.Air) {       // Second check if the tile is not air
                return true;                                                // If the tile is in bounds and not air, it's standable
            }
        }

        // None of the tiles are within bounds or standable, so return false
        return false;


    }


}
