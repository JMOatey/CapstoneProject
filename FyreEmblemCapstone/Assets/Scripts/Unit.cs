using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectedAction
{
	Attack,
	Wait,
	Move,
	Nothing
}

public class Unit : PlayerMove 
{
	public float Speed = 0;
	public int Health = 10;
	public int Attack = 2;
	public int AttackRange = 1;

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
				break;
			case SelectedAction.Move:
				MoveUpdate();
				break;
			case SelectedAction.Wait:
				HidePossibleMoves();
				TurnManager.Instance.EndTurn();
				break;
			case SelectedAction.Nothing:
				break;
		}
	}

	void OnMouseOver()
    {
        DisplayPossibleMoves();
    }

    void OnMouseExit()
    {
        HidePossibleMoves();
    }

	public void BeginTurn()
	{
		HasMoved = false;
		Turn = true;
	}

	public void EndTurn()
	{
		Turn = false;
	}
}
