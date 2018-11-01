using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFind {

    Path_AStar pathAStar;

    public PathFind() {
        


    }


    public void PathToDest(Tile currTile, Tile destTile) {

        pathAStar = new Path_AStar(currTile.world, currTile, destTile);

    }

    public void PathToAdj(Tile currTile, Tile destTile) {

        pathAStar = new Path_AStar(currTile.world, currTile, destTile);

    }







}
