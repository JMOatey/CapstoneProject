using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : PlayerUtility
{

	JumpMove JumpEnum = JumpMove.Regular;
	public Stack<Tile> Path = new Stack<Tile>();
	public bool Moving = false;
	public int MoveDistance = 5;
	public float JumpHeight = 2;
	public float MoveSpeed = 2;
	public float JumpVelocity = 4.5f;
	float HalfHeight = 0;
	public bool HasMoved = false;

	Vector3 velocity = new Vector3();
	Vector3 heading = new Vector3();
	Vector3 JumpTarget = new Vector3();

	// Use this for initialization
	protected void MoveStart ()
	{
		MoveInit();
	}
	
	// Update is called once per frame
	protected void MoveUpdate ()
	{
		if(!Turn)
		{
			return;
		}
		if(HasMoved){
			// TurnManager.Instance.EndTurn();
			return;
		}
		if(!Moving)
		{
			if(this.tag == "Enemy"){
				aiMove();
			}else{
				DisplayPossibleMoves();
				CheckMouse();
			}

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

	//Move to random tile
	void aiMove(){
		List<Tile> list = this.SelectableTiles;
		MoveToTile(list[Random.Range(0,list.Count)]);
	}

	public enum JumpMove
	{
		JumpingUp,
		FallingDown,
		MovingEdge,
		Regular

	}


	protected void MoveInit()
	{
		Tiles = GameObject.FindGameObjectsWithTag("Tile");

		HalfHeight = GetComponent<Collider>().bounds.extents.y;		
	}

	public void DisplayPossibleMoves()
	{
		if(SelectableTiles.Count == 0)
		{
			FindSelectableTiles();
			foreach(Tile tile in SelectableTiles)
			{
				tile.Selectable = true;
			}
		}
		else
		{
			foreach(Tile tile in SelectableTiles)
			{
				tile.Selectable = true;
			}
		}
	}

	public void HidePossibleMoves()
	{
		foreach(Tile tile in SelectableTiles)
		{
			tile.Reset();
		}
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
		foreach(GameObject tile in Tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors(JumpHeight);
		}
	}

	public void FindSelectableTiles()
	{
		ComputeAdjacencyLists();
		GetCurrentTile();

		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(CurrentTile);
		CurrentTile.Visited = true;

		while(process.Count > 0)
		{
			Tile t = process.Dequeue();
			
			SelectableTiles.Add(t);
			t.Selectable = true;

			if(t.Distance < MoveDistance)
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
	
	public void Move()
	{
		if(Path.Count > 0)
		{
			Tile t = Path.Peek();
			Vector3 target = t.transform.position;

			target.y += HalfHeight + t.GetComponent<Collider>().bounds.extents.y;
			
			if(Vector3.Distance(transform.position, target) >= 0.05f)
			{

				bool jump = transform.position.y != target.y;
				if(jump)
				{
					Jump(target);
				}
				else
				{
					CalculateHeading(target);
					SetHorizontalVelocity();
				}

				// Add animations here too
				transform.forward = heading;
				transform.position += velocity * Time.deltaTime;
			}
			else
			{
				transform.position = target;
				Path.Pop();
			}
		}
		else
		{
			RemoveSelectableTiles();
			Moving = false;
			HasMoved = true;
			GetCurrentTile();
			// TurnManager.EndTurn();
		}
	}

	protected void RemoveSelectableTiles()
	{
		if(CurrentTile != null)
		{
			CurrentTile.Current = false;
			CurrentTile = null;
		}
		foreach(Tile tile in SelectableTiles)
		{
			tile.Reset();
		}

		SelectableTiles.Clear();
	}

	void CalculateHeading(Vector3 target)
	{
		heading = target - transform.position;
		heading.Normalize();
	}

	void SetHorizontalVelocity()
	{
		velocity = heading * MoveSpeed;
	}

	//Jump Stuff
	#region
	void Jump(Vector3 target)
	{
		switch(JumpEnum)
		{
			case JumpMove.FallingDown:
				FallDown(target);
				break;
			case JumpMove.JumpingUp:
				JumpUp(target);
				break;
			case JumpMove.MovingEdge:
				MoveToEdge();
				break;
			default:
				PrepareJump(target);
				break;	
		}
	}

	void PrepareJump(Vector3 target)
	{
		float targetY = target.y;

		target.y = transform.position.y;

		CalculateHeading(target);

		if(transform.position.y > targetY)
		{
			JumpEnum = JumpMove.MovingEdge;

			JumpTarget = transform.position + (target - transform.position) / 2.0f;
		}
		else
		{
			JumpEnum = JumpMove.JumpingUp;

			velocity = heading * MoveSpeed / 3.0f;
			float difference = targetY - transform.position.y;

			velocity.y = JumpVelocity * (0.5f + difference / 2.0f);
		}
	}

	void FallDown(Vector3 target)
	{
		velocity += Physics.gravity * Time.deltaTime;

		if(transform.position.y <= target.y)
		{
			JumpEnum = JumpMove.Regular;

			Vector3 unit = transform.position;
			unit.y = target.y;
			transform.position = unit;

			velocity = new Vector3();
		}
	}

	void JumpUp(Vector3 target)
	{
		velocity += Physics.gravity * Time.deltaTime;

		if(transform.position.y > target.y)
		{
			JumpEnum = JumpMove.FallingDown;
		}
	}

	void MoveToEdge()
	{
		if(Vector3.Distance(transform.position, JumpTarget) >= 0.05f)
		{
			SetHorizontalVelocity();
		}
		else
		{
			JumpEnum = JumpMove.FallingDown;

			velocity /= 5.0f;
			velocity.y = 1.5f;
		}
	}
	#endregion
}
