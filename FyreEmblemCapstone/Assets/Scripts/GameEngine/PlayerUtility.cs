using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerUtility : MonoBehaviour 
{
	public bool Turn = false;
    public GameObject[] Tiles;
	public Tile CurrentTile; 
	public List<Tile> SelectableTiles = new List<Tile>();

    public void GetCurrentTile()
	{
		CurrentTile = GetTargetTile(gameObject);
		CurrentTile.Current = true;
	}

}

public static class PathingExtensions
{
    public static FindAvailableTiles(this List<Tile> graph, int distance, Tile currentTile)
    {
		foreach(GameObject tile in Tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors(JumpHeight);
		}
		Queue<Tile> process = new Queue<Tile>();
        graph.Clear();

		process.Enqueue(currentTile);
		current.Visited = true;

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
    }
}