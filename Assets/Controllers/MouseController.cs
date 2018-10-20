using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.EventSystems;




public class MouseController : MonoBehaviour {


    public GameObject circleCursorPrefab;

    TileType buildModeTile = TileType.Air;

    Vector3 currFramePosition;
    Vector3 lastFramePosition;

    Vector3 dragStartPosition;
    List<GameObject> dragPreviewGameObjects;

    



	// Use this for initialization
	void Start () {
        dragPreviewGameObjects = new List<GameObject>();
        

        //preload a bunch of gameobjects to avoid first time lag when creating a bunch at once
        SimplePool.Preload(circleCursorPrefab, 100);

        //Set Camera to the nomial surface - will change a lot in testing
        Camera.main.transform.Translate(50f, 75f, 0);
    }
	
	// Update is called once per frame
	void Update () {

        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //mousePosition is relative to screen (0-1,0-1), 
                                                                                         //  but can give this to the camera using a conversion gives us the true point in the world
                                                                                         //works fine in orthagraphic view if we are ignoring the z component
        currFramePosition.z = 0; //explicity set to 0, as weird stuff happens when the z gets set the same as the camera, and gets clipped


        UpdateDragging();
        UpdateCameraMovement();


        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // as we have moved the camera, we need to check mouse position again
        lastFramePosition.z = 0;

        

    }

    // Given a vector3 coordinate, returns the tile those coords rest on
    Tile GetTileAtWorldCoord(Vector3 coord)
    {
        //int x = Mathf.FloorToInt(coord.x);
       // int y = Mathf.FloorToInt(coord.y);

        //sprite anchor point defaults to the middle, so we will use
        //round to int. The centre of the (0,0) tile is at 0,0, which means the edges go from -0.49 to 0.50.
        //RoundToInt takes that range and makes it 0.    0.51 to 1.50 would round to 1
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);

        ////finds first gameObject of type "WorldController" in the hireachy
        //GameObject.FindObjectOfType<WorldController>();

        return WorldController.Instance.World.GetTileAt(x, y);

        
    }

    void UpdateDragging() {
        
        // If we are over a UI element, bail out!! Don't want to do anything here, 
        //Event System knows this, .current finds the current eventsystem, and IsPointer... checks if it's over UI elements

        //TODO: don't just bail out entirely, set flags to stop some activities, but keep others going
        //eg ending a drag over UI and changing tiles is fine, but starting a drag over UI isnt fine
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }



        //Start Drag
        if (Input.GetMouseButtonDown(0)) {
            dragStartPosition = currFramePosition;
        }


        int start_x = Mathf.RoundToInt(dragStartPosition.x);
        int end_x = Mathf.RoundToInt(currFramePosition.x);
        //in the case where drag right to left, flip so forloop works
        if (end_x < start_x) {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }

        int start_y = Mathf.RoundToInt(dragStartPosition.y);
        int end_y = Mathf.RoundToInt(currFramePosition.y);
        //in the case where drag top to bottom, flip so forloop works
        if (end_y < start_y) {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }


        //clean up old drag preview objects
        while (dragPreviewGameObjects.Count > 0) {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        //when held
        if (Input.GetMouseButton(0)) {
            //Display a preview of the drag area
            for (int x = start_x; x <= end_x; x++) {
                for (int y = start_y; y <= end_y; y++) {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null) {
                        //Display the building hint on top of this tile position (object, location, rotation)
                        GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.SetParent(this.transform, true);
                        dragPreviewGameObjects.Add(go);


                    }
                }
            }
        }


        //End Drag
        if (Input.GetMouseButtonUp(0)) {
            for (int x = start_x; x <= end_x; x++) {
                for (int y = start_y; y <= end_y; y++) {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null) {

                        //// Build a thing!
                        //// This sets the type of the tile to the currently chosen buildmode tile
                        //// E.g. if digging, the type is air
                        //t.Type = buildModeTile;

                        // Instead of that, we are going to make a job and add it to a jobqueue
                        // TODO: Only allow certain jobs
                        // Check for replication or overrides (dig on a dig, or dig on a fill)
                        // Check for legal placement (Dig on air not allowed etc)
                        // Probably get a buildmodeController or DesignateController or something and do it all there, just calling .BuildIt etc

                        Job job = new Job(t, buildModeTile);
                        WorldController.Instance.World.jobQueue.Enqueue(job);
                           

                        // Debug testing FIXME:
                        Debug.Log("Added job");
                        


                    }
                }
            }
        }
        


    }

    void UpdateCameraMovement() {
        //handle screen dragging
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) //checks if Right or middle mouse button is down
        {
            Vector3 diff = lastFramePosition - currFramePosition; //work out the difference between the 2 frames
            Camera.main.transform.Translate(diff); //move the difference

        }

        //camera zoom geometric? increase in zoom
        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        //hardcoding min and max sizes
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 50f);


    }

    public void SetMode_Fill() {
        buildModeTile = TileType.Dirt;
    }

    public void SetMode_Dig() {
        buildModeTile = TileType.Air;

    }

    //void UpdateCursor()
    //{

    //    //update the circle cursor positon
    //    Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
    //    if (tileUnderMouse != null)
    //    {
    //        Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
    //        circleCursor.transform.position = cursorPosition;
    //        circleCursor.SetActive(true);
    //    }
    //    else
    //    {
    //        circleCursor.SetActive(false);
    //    }

    //}
}
