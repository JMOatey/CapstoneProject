﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Lock this whenever executing an action
public enum SelectedAction
{
	Attack,
	Wait,
	Move,
	Nothing
}

public class Unit : PlayerMove
{
	public bool Finished = false;
	public float Speed = 0;
	public int Health = 10;

	public SelectedAction CurrentAction = SelectedAction.Nothing;

	void Start () {
		TurnManager.Instance.AddUnit(this);

		MoveStart();
	}
	
	void Update () {
		switch(CurrentAction)
		{
			case SelectedAction.Attack:
				HidePossibleMoves();
				AttackUpdate();
				break;
			case SelectedAction.Move:
				HideAttackableTiles();
				MoveUpdate();
				break;
			case SelectedAction.Wait:
				HideAttackableTiles();
				HidePossibleMoves();
				TurnManager.Instance.EndTurn();
				break;
			case SelectedAction.Nothing:
				break;
		}
	}
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

	//Move to random tile
	void aiMove(){
		List<Tile> list = this.SelectableTiles;
		MoveToTile(list[Random.Range(0,list.Count)]);
	}

	void OnMouseOver()
    {
        ShowEveryOption();
    }

    void OnMouseExit()
    {
        HideEverything();
    }

	public void BeginTurn()
	{
		GetCurrentTile();
		SelectableTiles.Clear();
		AttackableTiles.Clear();
		SelectableTiles.FindAvailableTiles(MoveDistance, CurrentTile, JumpHeight, Tiles);
		AttackableTiles.FindAvailableTiles(AttackRange, CurrentTile, JumpHeight, Tiles);
		HasMoved = false;
		Turn = true;
		HasAttacked = false;
	}

	public void EndTurn()
	{
		Turn = false;
		CurrentAction = SelectedAction.Nothing;
		foreach(GameObject tile in Tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.Reset();
		}
	}

	public void DisplayPossibleMoves()
	{
		if(SelectableTiles.Count == 0)
		{
			SelectableTiles.FindAvailableTiles(MoveDistance, CurrentTile, JumpHeight, Tiles);
		}
		foreach(Tile tile in SelectableTiles)
		{
			tile.Selectable = true;
		}
		CurrentTile.Occupied = true;		
	}

	public void HidePossibleMoves()
	{
		foreach(Tile tile in SelectableTiles)
		{
			tile.Reset();
		}
		foreach(Tile tile in AttackableTiles)
		{
			tile.Reset();
		}
		CurrentTile.Occupied = true;
	}

	public void ShowEveryOption()
	{
		if(TurnManager.Instance.CurrentUnit.CurrentAction == SelectedAction.Nothing)
		{
			DisplayPossibleMoves();
			DisplayAttackableTiles();
		}
		
	}

	public void HideEverything()
	{
		if(TurnManager.Instance.CurrentUnit.CurrentAction == SelectedAction.Nothing)
		{
			HideAttackableTiles();
			HidePossibleMoves();
		}
	}


	#region
	public int AttackDamage = 2;
	public int AttackRange = 1;
	public bool HasAttacked = false;

    // Start is called before the first frame update
    void AttackStart()
    {
        
    }

    // Update is called once per frame
    void AttackUpdate()
    {
		if(!HasAttacked)
		{
			Attack();
		}
        GetAttackableTiles();
		DisplayAttackableTiles();
    }

	void Attack()
	{
		if(Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				if(hit.collider.tag == "Player" || hit.collider.tag == "Enemy")
				{
					Unit unit = hit.collider.GetComponent<Unit>();
					Debug.Log(unit);
					unit.Health -= AttackDamage;
					HasAttacked = true;
				}
			}
		}
	}

    void GetAttackableTiles()
    {
		List<Tile> maxWalkDistance = SelectableTiles.Where(t => t.Distance == MoveDistance).ToList();
		foreach(Tile tile in maxWalkDistance)
		{
			AttackableTiles.FindAvailableTiles(AttackRange, tile, JumpHeight, Tiles);
		}
		if(this.tag == "Enemy")
		{

		}
		else
		{

		}
    }

	void DisplayAttackableTiles()
	{
		if(AttackableTiles.Count == 0)
		{
			GetAttackableTiles();
		}
		foreach(Tile tile in AttackableTiles)
		{
			tile.Attackable = true;
		}
	}

	void HideAttackableTiles()
	{
		foreach(Tile tile in AttackableTiles)
		{
			tile.Attackable = false;
		}
	}
	#endregion
}
