using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerUtility : MonoBehaviour 
{
	public bool Turn = false;
    public GameObject[] Tiles;
	public Tile CurrentTile; 
	public List<Tile> SelectableTiles = new List<Tile>();
	public float JumpHeight = 2;
	protected float HalfHeight = 0;
    protected List<Tile> AttackableTiles = new List<Tile>();
	public List<Tile> attackTiles{
		get {return AttackableTiles; }
	}


    public void GetCurrentTile()
	{
		CurrentTile = GetTargetTile(gameObject);
		CurrentTile.Occupied = true;
	}

    public Tile GetTargetTile(GameObject target)
	{
		RaycastHit hit;
		Tile tile = null;

		if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
		{
			tile = hit.collider.GetComponent<Tile>();
		}

		return tile;
	}
    // public static void FindAvailableTiles(this List<Tile> graph, int distance)
    // {
	// 	foreach(GameObject tile in Tiles)
	// 	{
	// 		Tile t = tile.GetComponent<Tile>();
	// 		t.FindNeighbors(jumpHeight);
	// 	}
	// 	Queue<Tile> process = new Queue<Tile>();
    //     graph.Clear();

	// 	process.Enqueue(currentTile);
	// 	CurrentTile.Visited = true;

	// 	while(process.Count > 0)
	// 	{
	// 		Tile t = process.Dequeue();
			
	// 		graph.Add(t);
	// 		if(t.Distance < distance)
	// 		{
	// 			foreach(Tile tile in t.AdjacencyList)
	// 			{
	// 				if(!tile.Visited)
	// 				{
	// 					tile.Parent = t;
	// 					tile.Visited = true;
	// 					tile.Distance = 1 + t.Distance;
	// 					process.Enqueue(tile);
	// 				}
	// 			}
	// 		}
	// 	}
    //     foreach(Tile t in graph)
    //     {
    //         t.Reset();
    //     }
    // }
}

public static class PathingExtensions
{
    public static void FindAvailableTiles(this List<Tile> graph, int distance, Tile currentTile, float jumpHeight, GameObject[] tiles)
    {
		foreach(GameObject tile in tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors(jumpHeight);
		}
		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(currentTile);
		currentTile.Visited = true;

		while(process.Count > 0)
		{
			Tile t = process.Dequeue();
			
			graph.Add(t);
			if(t.Distance < distance)
			{
				foreach(Tile tile in t.AdjacencyList)
				{
					if(!tile.Visited)
					{
						tile.Parent = t;
						tile.Visited = true;
						tile.Distance = 1 + t.Distance;
						process.Enqueue(tile);
					}
				}
			}
		}
        foreach(Tile t in graph)
        {
            t.Reset();
        }
        currentTile.Occupied = true;
    }

    public static void CalculateDistance(this List<Tile> graph, int distance, Tile currentTile, float jumpHeight, GameObject[] tiles)
    {
        foreach(GameObject tile in tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors(jumpHeight);
		}
		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(currentTile);
		currentTile.Visited = true;

		while(process.Count > 0)
		{
			Tile t = process.Dequeue();
			
			graph.Add(t);
			if(t.Distance < distance)
			{
				foreach(Tile tile in t.AdjacencyList)
				{
					if(!tile.Visited)
					{
						tile.Parent = t;
						tile.Visited = true;
						tile.Distance = 1 + t.Distance;
						process.Enqueue(tile);
					}
				}
			}
		}
    }
}