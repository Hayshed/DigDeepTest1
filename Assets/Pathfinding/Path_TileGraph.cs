using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph {

    // This class constructs a simple path-finding compatible graph
    // of our world. Each tile is a node. each WALKABLE neighbour
    // from a tile is linked vai an edge connection

    public Dictionary<Tile, Path_Node<Tile>> nodes; //Define a dictonary - a link between tiles and nodes

    public Path_TileGraph(World world) {

        Debug.Log("Path_TileGraph");

        // Loop through all tilse of the world
        // For each tile, create a node
        // Do we create nodes for non-floor tiles? NO!
        // Do we create nodes for tiles that are completely unwalkable? (i.e. walls?) NO!

        nodes = new Dictionary<Tile, Path_Node<Tile>>(); //Instanciate a dictonary - a link between tiles and nodes

        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {


                Tile t = world.GetTileAt(x, y);

                //if(t.MovementCost > 0) { // Tiles with a move cost of 0 are unwalkable
                    Path_Node<Tile> n = new Path_Node<Tile>();
                    n.data = t;
                    nodes.Add(t, n);

                //}

            }

        }

        Debug.Log("Path_TileGraph: Created " + nodes.Count + " nodes.");

        // Now loop through all nodes again
        // Create edges for neighbours

        int edgeCount = 0;

        foreach(Tile t in nodes.Keys) {                         // Go through whole dictonary

            Path_Node<Tile> n = nodes[t];

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            // Get a list of neighbours for the tile
            Tile[] neighbours = t.GetNeighbours(true); // NOTE: Some of the array spots could be null


            // If neighbour is walkable, create an edge to the relevent node
            for (int i = 0; i < neighbours.Length; i++) {       // for each neighbour...
                if(neighbours[i] != null && neighbours[i].MovementCost > 0) { // This neighbour exists and is empty (air)

                    // Now check if there is a block in the right place to stand on
                    // If the neighbour tile is not standable, we skip to the next neighbour without building an edge
                    if(neighbours[i].IsStandable() == false) {
                        continue;
                    }


                    // But first make sure we are not clipping a diagonal or trying to squeeze inappropiately
                    if (IsClippingCorner(t, neighbours[i]) ) {
                        continue;       // Skip to the next neighbour without building an edge
                    }

                    Path_Edge<Tile> e = new Path_Edge<Tile>();  // Create a new edge
                    e.cost = neighbours[i].MovementCost;        // Set the movement cost
                    e.node = nodes[neighbours[i]];              // Set the node

                    edges.Add(e);                               // Add this new edge to our temporary (and growable) list of edges

                    edgeCount++;                                // Debug counter
                }
            }

            n.edges = edges.ToArray();                          // Give the node the list of edges

            
        }

        Debug.Log("Path_TileGraph: Created " + edgeCount + " edges.");

    }

    bool IsClippingCorner(Tile curr, Tile neigh) {
        // if the movement from curr to neigh is diagonal(e.g NE)
        // then check to make sure we aren't clipping (E.g. that N and E are both walkable



        if(Mathf.Abs(curr.X - neigh.X) + Mathf.Abs(curr.Y - neigh.Y) == 2) {
            // We are diagonal
            int dX = curr.X - neigh.X;
            int dY = curr.Y - neigh.Y;

            if(curr.world.GetTileAt(curr.X - dX, curr.Y).MovementCost == 0) {
                // East or West is unwalkable, therefore this would be clipped movement
                return true;
            }

            if (curr.world.GetTileAt(curr.X, curr.Y - dY).MovementCost == 0) {
                // North or South is unwalkable, therefore this would be clipped movement
                return true;
            }

        }

        return false;
    }

	
}
