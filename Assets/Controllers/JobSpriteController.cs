using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {

    //Sprite digSprite;
    //Sprite fillSprite;

    Dictionary<Job, GameObject> jobGameObjectMap;
    Dictionary<string, Sprite> spriteMap;

    // Use this for initialization
    void Start () {

        LoadSprites();

        jobGameObjectMap = new Dictionary<Job, GameObject>();

        JobQueue j = WorldController.Instance.World.jobQueue;
            
        j.RegisterJobCreationCallback(JobCreated);

    }

    // Makes Visual Game Objects when job is created
    void JobCreated(Job job) {
        //Make a visual object and link it to the Job
        GameObject job_go = new GameObject();
        // Add to dictonary
        jobGameObjectMap.Add(job, job_go);
        // Set name, location and parent of the visual object
        job_go.name = "JOB_" + job.JobType + "_" + job.BuildTile.X + "_" + job.BuildTile.Y;
        job_go.transform.position = new Vector3(job.BuildTile.X, job.BuildTile.Y, 0);
        job_go.transform.SetParent(this.transform, true);
        // Set up the sprites 
        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>(); //- add sprite renderer

        // Selects correct job symbol depending on job type
        if (job.JobType == TileType.Air) {
            sr.sprite = GetSpriteFromName("DigPick"); 
        }
        else {
            sr.sprite = GetSpriteFromName("Fill");
        }
        sr.sortingLayerName = "Jobs";


        // Get rid of jobs once they are done. Do this by running onJobEnded
        job.RegisterJobCompleteCallback(onJobEnded);
        job.RegisterJobCanceledCallback(onJobEnded);

    }

    void LoadSprites() {
        spriteMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Blocks/");
        Debug.Log("Loading Resource:");
        foreach (Sprite s in sprites) {
            Debug.Log(s);
            spriteMap[s.name] = s;
        }
    }

    public Sprite GetSpriteFromName(string name) {
        return spriteMap[name];
    }




    // Cleans up Visual Game Objects when job is ended
    // Job is already removed from jobqueue when it gets pulled (dequeued), so just need to get rid of the job Visual object
    // Removing callbacks from job for completeness
    void onJobEnded(Job job) {
        GameObject job_go = jobGameObjectMap[job];

        job.UnRegisterJobCanceledCallback(onJobEnded);
        job.UnRegisterJobCompleteCallback(onJobEnded);

        Destroy(job_go);
    }


}
