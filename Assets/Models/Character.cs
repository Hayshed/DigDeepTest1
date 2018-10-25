using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character {

    Tile currTile;              // The current tile the character resides on
    Tile destTile;              // The tile they want to pathfind to
    Tile nextTile;              // The next tile to move to on the path to the dest tile
    Job myJob;                    // Their current job they want to do
    float speed;                // How many tiles they will move per second
    float movementPercentage;
    Path_AStar pathAStar;

    Action<Character> cbCharacterChanged;

    public float X {
        get {
            return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
        }
    }

    public float Y {
        get {
            return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage);
        }
    }


    // Constructor
    public Character(Tile tile, float speed = 4f) {
        this.currTile = destTile = nextTile = tile;
        this.speed = speed;

        movementPercentage = 0;

    }


    // World calls this to update the character
    public void Update(float deltaTime) {

        Update_doJob(deltaTime);

        Update_move(deltaTime);


        if (cbCharacterChanged != null) {   // Let other objects know that we have changed -- TODO: don't want to run every tick???
            cbCharacterChanged(this);
        }

    }

    // Move the character to their destination tile
    // TODO: combine/ restructure Update_move and Update_doJob, once should flow from the other, or maybe both from something else, there's double ups of checks
    void Update_move(float deltaTime) {

        if( currTile == destTile) {
            pathAStar = null;
            return; //We'll already there, don't move
        }
        if (nextTile == null || nextTile == currTile) {
            // Get the next tile from the pathfinder
            if (pathAStar == null || pathAStar.Length() == 0) {
                //Generate a path to our destination
                pathAStar = new Path_AStar(currTile.world, currTile, destTile);  // This will calculate a path from curr to dest.
                if (pathAStar.Length() == 0) {
                    Debug.LogError("Path_AStar returned no path to destination!");
                    //FIXME: job should maybe be re-enqueued instead??
                    AbandonJob();
                    pathAStar = null;
                    return;
                }
            }
            // Grab the next waypoint from the pathing system
            nextTile = pathAStar.Dequeue();

            if (nextTile == currTile) {
                Debug.LogError("Update_DoMovement -- nextTile is currTile?"); // Fixed by not giving the current tile as the first nextTile
                
            }
        }


        // not at the job site, so lets move our speed
        // FIXME: Should only calculate the distance once // use predefined values for diagonals
        float totalDist = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(currTile.X - nextTile.X), 2) + Mathf.Pow(Mathf.Abs(currTile.Y - nextTile.Y), 2));
        float distThisTick = speed * deltaTime;
        movementPercentage = movementPercentage + distThisTick / totalDist;

        // Check if we made it
        if (movementPercentage >= 1) {
            currTile = nextTile;   // set current tile to the next tile. We made it and we don't want to move more. nextTile will be set anew when picking up a new job
            movementPercentage = 0; // Reset to 0 ready for next move - TODO: not worrying about overflow for time spent on job yet
        }



    }

    void Update_doJob(float deltaTime) {
        // Get a job if we don't already have one
        if (myJob == null) {

            myJob = WorldController.Instance.World.jobQueue.Dequeue();
            if (myJob != null) {        // Check that a real job was pulled from the jobqueue
                destTile = myJob.buildTile;     // set the job tile as my destination
                myJob.RegisterJobCanceledCallback(OnJobEnded);  // we want to know if the job is completed or canceled, so we can discard it and do something else
                myJob.RegisterJobCompleteCallback(OnJobEnded);
            }
            else {
                return; // Pull out early if the job pull failed, we don't want to try to run nothing!
            }
        }

        // Check if the character is actually at the job site
        if (currTile == destTile) {
            if(myJob != null) {
                myJob.DoWork(deltaTime);
            }

            
        }

    }

    public void AbandonJob() {
        nextTile = destTile = currTile;
        pathAStar = null;
        currTile.world.jobQueue.Enqueue(myJob); //TODO: Puts the job back on the jobQueue, might not be best idea??
        myJob = null;

    }


    // Job is completed or canceled - get rid of job correctly
    void OnJobEnded(Job j) {

        // Check that we are being told about the right job
        if(j != myJob) {
            Debug.LogError("Character being told about job that isn't his - probably forgot tounregsiter something");
        }

        myJob = null;   // set my job to null
        
    }

    public void RegisterOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged -= cb;
    }

}
