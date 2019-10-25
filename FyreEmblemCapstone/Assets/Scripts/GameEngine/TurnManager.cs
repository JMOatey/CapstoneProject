using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TurnManager : MonoBehaviour 
{

	private static TurnManager _instance;
	public static TurnManager Instance
	{
		get
        {
            return _instance;
        }
	}

	private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

	Dictionary<string, List<Unit>> Units = new Dictionary<string, List<Unit>>();
	// Queue<string> TurnQueue = new Queue<string>();
	Queue<Unit> UnitQueue = new Queue<Unit>();
	public Queue<Unit> UQ{
		get {return UnitQueue; }
	}
	public Unit CurrentUnit;
	public List<Tile> Board;
	public int Turn = 1;

	void Start () 
	{
		// Board = C
		StartTurn();
	}
	
	void Update () {
	// 	if(TeamQueue.Count == 0)
	// 	{
	// 		InitTeamQueue();
	// 	}
	// 	if(Input.GetMouseButtonUp(0))
	// 	{
	// 		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

	// 		RaycastHit hit;
	// 		if(Physics.Raycast(ray, out hit))
	// 		{
	// 			if(Instance.UnitQueue.Peek())
	// 			{
	// 				if(Instance.CurrentUnit)
	// 				{
	// 					Instance.CurrentUnit.HidePossibleMoves();
	// 					Debug.Log("Should be hiding!");
	// 				}
	// 				Unit player = hit.collider.GetComponent<Unit>();

	// 				Instance.CurrentUnit = player;
	// 			}
	// 		}
	// 	}
	}

	// static void InitUnitQueue()
	// {
	// 	List<Unit> teamList = Instance.Units[Instance.UnitQueue.Peek()];

	// 	foreach(Unit unit in teamList)
	// 	{
	// 		Instance.UnitQueue.Enqueue(unit);
	// 	}

	// 	Instance.StartTurn();
	// }

	

	public void StartTurn()
	{
		Instance.UnitQueue.OrderBy( u => u.Speed);
		// foreach(PlayerAction pa in Units[TurnQueue.Peek()])
		// {
		// 	pa.BeginTurn();
		// }
		if(Instance.UnitQueue.Count > 0)
		{
			Instance.CurrentUnit = Instance.UnitQueue.Peek();
			Instance.CurrentUnit.BeginTurn();
			Debug.Log(Instance.CurrentUnit.gameObject.name);
			if(CurrentUnit.tag == "Enemy"){
				AI.aiAction(TurnManager.Instance);
			}
		}
	}

	public void EndTurn()
	{
		// foreach(PlayerAction pa in Units[TurnQueue.Peek()])
		// {
		// 	pa.EndTurn();
		// }
		Unit unit = Instance.UnitQueue.Dequeue();
		unit.EndTurn();
		unit.Finished = true;
		Instance.UnitQueue.Enqueue(unit);


		if(Instance.UnitQueue.Count > 0 && Instance.UnitQueue.Peek().Finished)
		{
			foreach(Unit u in Instance.UnitQueue)
			{
				u.Finished = false;
			}
			Instance.UnitQueue.OrderBy(u => u.Speed);
			Instance.Turn++;
		}
		StartTurn();
	}

	public void AddUnit(Unit unit)
	{
		List<Unit> list;

		if(!Instance.Units.ContainsKey(unit.tag))
		{
			list = new List<Unit>();
			Instance.Units[unit.tag] = list;

			// if(!Instance.TurnQueue.Contains(unit.tag))
			// {
			// 	Instance.TurnQueue.Enqueue(unit.tag);
			// }
		}
		else
		{
			list = Instance.Units[unit.tag];
		}
		list.Add(unit);
		Instance.UnitQueue.Enqueue(unit);
	}

	public void SelectAttack()
	{
		Instance.CurrentUnit.CurrentAction = SelectedAction.Attack;
	}

	public void SelectMove()
	{
		Instance.CurrentUnit.CurrentAction = SelectedAction.Move;
	}

	public void SelectWait()
	{
		Instance.CurrentUnit.CurrentAction = SelectedAction.Wait;
	}

}
