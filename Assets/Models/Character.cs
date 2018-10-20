using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

    Tile currTile;              // The current tile the character resides on
    Tile destTile;              // The tile they want to pathfind to
    Job job;                    // Their current job they want to do
    float speed;                // How many tiles they will move per second


	// Constructor
    public Character(Tile currTile, float speed) {
        this.currTile = currTile;
        this.speed = speed;

    }

    

}
