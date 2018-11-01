using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Character {

    Tile currTile;              // The current tile the character resides on
    Tile destTile;              // The tile they want to pathfind to
    Tile nextTile;              // The next tile to move to on the path to the dest tile
    Tile adjTile;                // Adjacent tile to job tile
    Tile jobTile;               // Tile that is changed by the job
    Job myJob;                    // Their current job they want to do
    float speed;                // How many tiles they will move per second
    float movementPercentage;
    
    PathFind pathfinder;
    Path_AStar mainPath;
    List<Path_AStar> potentialPathList;
    Dictionary<Path_AStar, Tile> pathToDestTileMap;

    

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
        this.currTile = nextTile = tile;
        this.speed = speed;
        destTile = null;
        

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
                jobTile = myJob.buildTile;     // set the job tile as my jobtile
                myJob.RegisterJobCanceledCallback(OnJobEnded);  // we want to know if the job is completed or canceled, so we can discard it and do something else
                myJob.RegisterJobCompleteCallback(OnJobEnded);

                // It's a new job, reset everything as we'll have to work out pathfinding again
                destTile = null;
                nextTile = currTile; 

            }
        }

        // At this stage I should have a job
        // Am I at the job site?

        if (destTile != null && currTile == destTile) {
            // If I'm at the job site, work on the job
            myJob.DoWork(deltaTime);
        }

        else {
            // Do I have my next step (nextTile) ?
            // If I don't have my next step or are on it
            if (nextTile == null || currTile == nextTile) {
                // Check if I have a path to follow
                // If I don't have a path...
                if (mainPath == null || mainPath.Length() == 0) {


                    // Check all neighbours 
                    // Get a list of all the neighbouring tiles to the jobTile
                    Tile[] neighbourList = jobTile.GetNeighbours(true);
                    // Local lists to hold possilbe paths and tile destinations. 
                    // Won't need these after we have worked out the one we want
                    potentialPathList = new List<Path_AStar>();
                    pathToDestTileMap = new Dictionary<Path_AStar, Tile>();


                    // Check each tile adjacent to the job tile, and if it's standable generate a path to it and add that path to the list of potental paths
                    foreach (Tile t in neighbourList) {
                        if (t.IsStandable() == true) {
                            Path_AStar PTemp = new Path_AStar(t.world, currTile, t);
                            // check that there is a legitimate path before adding it to the list of potential paths
                            if (PTemp.Length() != 0) {
                                potentialPathList.Add(PTemp);
                                pathToDestTileMap.Add(PTemp, t);
                            }
                        }
                    }

                    // Check that we have at least one potential path
                    // If we don't, bail out as pathing is impossible
                    if (potentialPathList.Count <= 0) {
                        //Debug.LogError("No Potential Paths found");
                        AbandonJob();   // Pull out of job and put job back on queue
                        mainPath = null;
                        potentialPathList.Clear();
                        pathToDestTileMap.Clear();
                        return;
                    }


                    // We should now have at least one potential path, so we will see which one is the shortest
                    // Set first on list at the shortest, and check each path agaisnt it, a shorter path becoming the new shortest
                    Path_AStar shortestPath = potentialPathList.First();

                    foreach (Path_AStar path in potentialPathList) {
                        if (path.Length() < shortestPath.Length()) {
                            shortestPath = path;
                        }
                    }

                    // We now have a path to use that will take us to a legitmate building spot\
                    // And also the tile we are trying to get to

                    mainPath = shortestPath;
                    destTile = pathToDestTileMap[shortestPath];

                }
                // At this stage we should have a path
                nextTile = mainPath.Dequeue();
            }
            // At this stage we should have a nextTile
            // If I'm not at the jobsite, move towards it
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

        myJob.UnRegisterJobCompleteCallback(OnJobEnded);
        myJob.UnRegisterJobCanceledCallback(OnJobEnded);

        nextTile = currTile;
            destTile = null;
        mainPath = null;
        WorldController.Instance.World.jobQueue.Requeue(myJob); //TODO: Puts the job back on the jobQueue, might not be best idea??
        myJob = null;

    }


    // Job is completed or canceled - get rid of job correctly
    void OnJobEnded(Job j) {

        // unregister from the job callback, the job is done or canceled we no longer care about it
        j.UnRegisterJobCompleteCallback(OnJobEnded);
        j.UnRegisterJobCanceledCallback(OnJobEnded);

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
