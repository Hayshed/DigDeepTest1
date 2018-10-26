using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character {

    Tile currTile;              // The current tile the character resides on
    Tile destTile;              // The tile they want to pathfind to
    Tile nextTile;              // The next tile to move to on the path to the dest tile
    Tile adjTile;                // Adjacent tile to job tile
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

        // Check if I have a job
        // If I don't have a job..
        if( myJob == null) {
            // Try to pull a job
            myJob = WorldController.Instance.World.jobQueue.Dequeue();
            // Did that pull succeed?
            // If I still don't have a job
            if (myJob == null) {
                // Pullout, I don't have a job and can't get one. I don't want to do anything else
                return;
            }
            // If I did get a job...
            else {
                destTile = myJob.buildTile;     // set the job tile as my destination
                myJob.RegisterJobCanceledCallback(OnJobEnded);  // we want to know if the job is completed or canceled, so we can discard it and do something else
                myJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }

        // At this stage I should have a job

        // Am I at the job site?
        if (currTile == destTile) {
            // If I'm at the job site, work on the job
            myJob.DoWork(deltaTime);
        }
        // If I'm not at the job site I should move towards it
        else {
            // Do I have my next step (nextTile) ?
            // If I don't have my next step or are on it
            if (nextTile == null || currTile == nextTile) {
                // Check if I have a path to follow
                // If I don't have a path...
                if (pathAStar == null || pathAStar.Length() == 0) {
                    //Generate a path to our destination
                    pathAStar = new Path_AStar(currTile.world, currTile, destTile);  // This will calculate a path from curr to dest.


                    // TODO: Check to be added in future - adjacent jobs supported ONLY
                    //destTile = pathAStar.PathToAdjacentTile(); // Makes the path go to an adjacent tile to the job tile rather than the job tile


                    // Check that it returned a legit path
                    // If the path isn't legit
                    if (pathAStar.Length() == 0) {
                        //Debug.LogError("Path_AStar returned no path to destination!");
                        AbandonJob();   // Pull out of job and put job back on queue
                        pathAStar = null;
                        return;
                    }
                }
                // At this stage we should have a path
                nextTile = pathAStar.Dequeue();
            }
            // At this stage we should have a nextTile

            // Move to the next Tile
            MoveToNextTile(deltaTime);
        }


        if (cbCharacterChanged != null) {   // Let other objects know that we have changed -- TODO: don't want to run every tick???
            cbCharacterChanged(this);
        }

    }

    public void MoveToNextTile(float deltaTime) {
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
   
    public void AbandonJob() {

        nextTile = currTile;
            destTile = null;
        pathAStar = null;
        WorldController.Instance.World.jobQueue.Requeue(myJob); //TODO: Puts the job back on the jobQueue, might not be best idea??
        myJob = null;

    }


    // Job is completed or canceled - get rid of job correctly
    void OnJobEnded(Job j) {

        // unregister from the job callback, the job is done or canceled we no longer care about it
        j.UnRegisterJobCompleteCallback(OnJobEnded);
        j.UnRegisterJobCompleteCallback(OnJobEnded);

        // Check that we are being told about the right job
        if (j != myJob) {
            Debug.LogError("Character being told about job that isn't his - probably forgot to unregsiter something");
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
