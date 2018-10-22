using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job  {

    public Tile buildTile;         // Tile that will be changed by the job      
    TileType jobType;         // The TileType that will be placed there
    float jobTime;          // Nomial time it takes to complete job


    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    // Constructor
    public Job(Tile buildTile, TileType jobType, float jobTime = 1f) {
        this.BuildTile = buildTile;
        this.JobType = jobType;
        this.jobTime = jobTime;
    }

    public TileType JobType {
        get {
            return jobType;
        }

        set {
            jobType = value;
        }
    }
    public Tile BuildTile {
        get {
            return buildTile;
        }

        set {
            buildTile = value;
        }
    }

    public void DoWork(float workTime) {
        // Decrease the amount of time left on the job by the amount of time put in
        jobTime -= workTime;

        // Check if job is complete
        if(jobTime <= 0) {
            BuildTile.Type = JobType;           // Build the thing on the tile - change the tile type to "Air" or "Stone" etc
            cbJobComplete(this);
        }
    }

    public void RegisterJobCompleteCallback(Action<Job> cb) {

        cbJobComplete += cb;
    }

    public void UnRegisterJobCompleteCallback(Action<Job> cb) {

        cbJobComplete -= cb;
    }

    public void RegisterJobCanceledCallback(Action<Job> cb) {
    
        cbJobCancel += cb;
    }

    public void UnRegisterJobCanceledCallback(Action<Job> cb) {

        cbJobCancel -= cb;
    }

    public void CancelJob() {
        if (cbJobCancel != null) {
            cbJobCancel(this);
        }
    }



}
