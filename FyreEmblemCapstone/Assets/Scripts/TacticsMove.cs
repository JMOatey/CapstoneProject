using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour 
{
	List<Tile> selectableTiles = new List<Tile>();
	GameObject[] tiles;

	Stack<Tile> path = new Stack<Tile>();
	Tile currentTile;

	public int move = 5;
	public float jumpHeight = 2;
	public float moveSpeed = 2;

	float halfHeight = 0;


	Vector3 velocity = new Vector3();
	Vector3 heading = new Vector3();
	protected void Init()
	{
		tiles = GameObject.FindGameObjectsWithTag("Tile");

		halfHeight = GetComponent<Collider>().bounds.extents.y;
	}

	public void GetCurrentTile()
	{
		currentTile = GetTargetTile(gameObject);
		currentTile.Current = true;
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

	public void ComputeAdjacencyLists()
	{
		foreach(GameObject tile in tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors(jumpHeight);
		}
	}

	public void FindSelectableTiles()
	{
		ComputeAdjacencyLists();
		GetCurrentTile();

		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(currentTile);
		currentTile.Visited = true;

		while(process.Count > 0)
		{
			Tile t = process.Dequeue();
			
			selectableTiles.Add(t);
			t.Selectable = true;

			if(t.Distance < move)
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
