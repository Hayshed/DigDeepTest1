using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Node<T> { // When instanciated, Path_Node expects some kind of object/datatype to be specificed - T - Is a placeholder -- Path_Node<Tile> SomeTileNodeThing

    public T data;

    public Path_Edge<T>[] edges;  //Nodes leading OUT from this node.
	
}
