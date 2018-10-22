using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    // A 2D array containing all the tiles that will make up this world
    Tile[,] tiles;

    //The tile width and height of the world
    public int Width { get; protected set; }
    public int Height { get; protected set; }


    Action<Character> cbCharacterCreated;

    public JobQueue jobQueue;

    public List<Character> characterList;

    /// <summary>
    /// constructor for making a world,
    /// <param name="width"> the width in tiles</param>
    /// <param name="height"> the height in tiles</param>
    /// </summary>
    public World(int width = 100, int height = 100){

        jobQueue = new JobQueue();

        Width = width;
        Height = height;

        // tells the 2d array of tiles to be a certain width and height and then
        // fills the array using nested for loops, passing each tile it's location and the
        // world ref
        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);

            }
        }

        // Make a list to hold the characters
        characterList = new List<Character>();

        // Make one character
        Character c = CreateCharacter(GetTileAt(Width / 2, 1 + 3 * Height / 4));
        CreateCharacter(GetTileAt(Width / 2 + 1, 1 + 3 * Height / 4));
        CreateCharacter(GetTileAt(Width / 2 + 2, 1 + 3 * Height / 4));
        CreateCharacter(GetTileAt(Width / 2 + 4, 1 + 3 * Height / 4));
        CreateCharacter(GetTileAt(Width / 2 + 5, 1 + 3 * Height / 4));
        CreateCharacter(GetTileAt(Width / 2 + 6, 1 + 3 * Height / 4));

    }
    
    // World updates all relevant objects - updates all characters currently, so they can pull jobs and do work
    public void Update(float deltaTime) {

        foreach (Character c in characterList) {
            c.Update(deltaTime);
        }

    }

    

    public Tile GetTileAt(int x, int y)
    {

        
        Tile t;
        //checking if out of bounds
        if (x >= Width || x < 0 || y >= Height || y < 0) //NB: tiles go from 0 to 99, there are 100, but there should never be an 100th tile, hence checking for Width and above or below zero
        {
            //Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        t = tiles[x, y];
        //Debug.LogError("Tile (" + x + "," + y + ") is out of array range.");

        if (t == null) {
            return null;

        }

        return t;

    }

    public void RandomiseTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile t = GetTileAt(x, y);

                // randomiser between dirt and air
                if ( UnityEngine.Random.Range(0, 2) == 0)
                {
                    t.Type = TileType.Air;
                }
                else
                {
                    t.Type = TileType.Dirt;
                }

                
                

            }

        }
    }

    public void InitialiseTiles() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Tile t = GetTileAt(x, y);
                if (y <=  3 * Height / 4) {
                    t.Type = TileType.Dirt;
                }
                else {
                    t.Type = TileType.Air;
                }


                
                
            }
        }
    }

    public Character CreateCharacter(Tile t) {

        Debug.Log("Creating character...");
        Character c = new Character(t);         // create a new character
        characterList.Add(c);                   // Add it to the character list to keep track of it

        // Run callback to tell other objects that a character has been created
        if (cbCharacterCreated != null) {
            cbCharacterCreated(c);
            Debug.Log("sending characterCreated callback");
        }
        return c;
    }

    public void RegisterCharacterCreated(Action<Character> callbackfunc) {
        cbCharacterCreated += callbackfunc;
    }

    public void UnregisterCharacterCreated(Action<Character> callbackfunc) {
        cbCharacterCreated -= callbackfunc;
    }




}
