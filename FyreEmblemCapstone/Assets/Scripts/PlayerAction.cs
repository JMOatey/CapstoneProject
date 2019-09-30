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

public class PlayerAction : PlayerMove 
{
	public SelectedAction CurrentAction = SelectedAction.Nothing;

	void Start () {
		TurnManager.AddUnit(this);

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
				TurnManager.EndTurn();
				break;
			case SelectedAction.Nothing:
				break;
		}
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
