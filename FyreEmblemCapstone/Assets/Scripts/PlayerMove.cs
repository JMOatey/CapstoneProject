using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{

	// Use this for initialization
	protected void MoveStart ()
	{
		MoveInit();
	}
	
	// Update is called once per frame
	protected void MoveUpdate ()
	{
		Debug.DrawRay(transform.position, transform.forward);

		if(!Turn)
		{
			return;
		}
		if(!Moving)
		{
			if(SelectableTiles.Count < 1)
			{
				FindSelectableTiles();
			}
			CheckMouse();
		}
		else
		{
			Move();
		}
	}
	
	void CheckMouse()
	{
		if(Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				if(hit.collider.tag == "Tile")
				{
					Tile t = hit.collider.GetComponent<Tile>();
					if(t.Selectable)
					{
						MoveToTile(t);
					}
				}
			}
		}
	}

	public void MoveToTile(Tile tile)
	{
		Path.Clear();
		tile.Target = true;
		Moving = true;

		Tile next = tile;
		while(next != null)
		{
			Path.Push(next);
			next = next.Parent;
		}
	}
}
