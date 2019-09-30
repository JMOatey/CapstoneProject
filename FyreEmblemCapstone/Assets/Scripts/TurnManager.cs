using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TurnManager : MonoBehaviour {

	static Dictionary<string, List<PlayerAction>> Units = new Dictionary<string, List<PlayerAction>>();
	static Queue<string> TurnQueue = new Queue<string>();
	// static Queue<PlayerAction> TeamQueue = new Queue<PlayerAction>();
	static PlayerAction CurrentUnit;

	// Use this for initialization
	void Start () {
		StartTurn();
	}
	
	// Update is called once per frame
	void Update () {
		// if(TeamQueue.Count == 0)
		// {
		// 	InitTeamQueue();
		// }

		if(Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				if(hit.collider.tag == TurnQueue.Peek())
				{
					if(CurrentUnit)
					{
						CurrentUnit.HidePossibleMoves();
						Debug.Log("Should be hiding!");
					}
					PlayerAction player = hit.collider.GetComponent<PlayerAction>();

					CurrentUnit = player;
				}
			}
		}
	}

	// static void InitTeamQueue()
	// {
	// 	List<PlayerAction> teamList = Units[TurnQueue.Peek()];

	// 	foreach(PlayerAction unit in teamList)
	// 	{
	// 		TeamQueue.Enqueue(unit);
	// 	}

	// 	StartTurn();
	// }

	

	public static void StartTurn()
	{
		foreach(PlayerAction pa in Units[TurnQueue.Peek()])
		{
			pa.BeginTurn();
		}
		// if(TeamQueue.Count > 0)
		// {
		// 	CurrentUnit = TeamQueue.Peek();
		// 	CurrentUnit.BeginTurn();
		// }
	}

	public static void EndTurn()
	{
		foreach(PlayerAction pa in Units[TurnQueue.Peek()])
		{
			pa.EndTurn();
		}
		// PlayerAction unit = TeamQueue.Dequeue();
		// unit.EndTurn();

		// if(TeamQueue.Count > 0)
		// {
		// 	StartTurn();
		// }
		// else
		// {
		string team = TurnQueue.Dequeue();
		TurnQueue.Enqueue(team);
		StartTurn();
		// InitTeamQueue();
		// }
	}

	public static void AddUnit(PlayerAction unit)
	{
		List<PlayerAction> list;

		if(!Units.ContainsKey(unit.tag))
		{
			list = new List<PlayerAction>();
			Units[unit.tag] = list;

			if(!TurnQueue.Contains(unit.tag))
			{
				TurnQueue.Enqueue(unit.tag);
			}
		}
		else
		{
			list = Units[unit.tag];
		}
		list.Add(unit);
	}

	public void SelectAttack()
	{
		CurrentUnit.CurrentAction = SelectedAction.Attack;
	}

	public void SelectMove()
	{
		CurrentUnit.CurrentAction = SelectedAction.Move;
	}

	public void SelectWait()
	{
		CurrentUnit.CurrentAction = SelectedAction.Wait;
	}
}
