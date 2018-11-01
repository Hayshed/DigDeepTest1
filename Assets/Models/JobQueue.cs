using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
/// <summary>
/// Wrapper for a queue of jobs
/// </summary>
public class JobQueue  {

    
    Queue<Job> jobQueue;        // queue of jobs

    Action<Job> cbJobCreated;   // holds all the functions that get run when a job is created. If other classes need to be informed about job creation, they will register their 
                                // functions in this


    // Constructor
    public JobQueue() {
        jobQueue = new Queue<Job>();
    }

    // Adds a job to the job queue
    public void Enqueue(Job job) {
        jobQueue.Enqueue(job);      // adds a job to the queue
        if (cbJobCreated != null) {
            cbJobCreated(job);          // lets others (JobSpriteController) know that a job has been made that needs to be displayed
        }
        
    }


    //Adds a job to the queue that was previously on it - we don't want to call job created as it was not created, just taken off and put back on the queue
    public void Requeue(Job job) {
        jobQueue.Enqueue(job);
    }

    // Returns a job, taking it out of the queue
    public Job Dequeue() {
        if (jobQueue.Count > 0) {
            return jobQueue.Dequeue();
        }
        return null; 
    }

    // Lets functions be registered for when a job is created
    public void RegisterJobCreationCallback(Action<Job> cb) {
        cbJobCreated += cb;

    }
	
}
