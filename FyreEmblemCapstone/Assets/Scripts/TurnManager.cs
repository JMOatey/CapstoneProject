using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	static Dictionary<string, List<TacticsMove>> Units = new Dictionary<string, List<TacticsMove>>();
	static Queue<string> TurnQueue = new Queue<string>();
	static Queue<TacticsMove> TeamQueue = new Queue<TacticsMove>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(TeamQueue.Count == 0)
		{
			InitTeamQueue();
		}
	}

	static void InitTeamQueue()
	{
		List<TacticsMove> teamList = Units[TurnQueue.Peek()];

		foreach(TacticsMove unit in teamList)
		{
			TeamQueue.Enqueue(unit);
		}
		StartTurn();
	}

	public static void StartTurn()
	{
		if(TeamQueue.Count > 0)
		{
			TeamQueue.Peek().BeginTurn();
		}
	}

	public static void EndTurn()
	{
		TacticsMove unit = TeamQueue.Dequeue();
		unit.EndTurn();

		if(TeamQueue.Count > 0)
		{
			StartTurn();
		}
		else
		{
			string team = TurnQueue.Dequeue();
			TurnQueue.Enqueue(team);
			InitTeamQueue();
		}
	}

	public static void AddUnit(TacticsMove unit)
	{
		List<TacticsMove> list;

		if(!Units.ContainsKey(unit.tag))
		{
			list = new List<TacticsMove>();
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
}
