using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Account;


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

	public Dictionary<string, List<Unit>> Units = new Dictionary<string, List<Unit>>();
	// Queue<string> TurnQueue = new Queue<string>();
	public Queue<Unit> UnitQueue = new Queue<Unit>();
	public Unit CurrentUnit;
	public List<Tile> Board;
	public int Turn = 1;

   /* private void OnGUI()
    {
        int offset = 30;
        int initX = 85;
        int initY = 80;

        //Turn Queue Label
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(initX, initY, 100, 50), "Turn Queue \n ----------------");

        //Turn Queue Data
        for(int i = 0; i < Instance.UnitQueue.Count; i++)
        {   //Current Unit
            if(Instance.UnitQueue.ElementAt<Unit>(i) == Instance.CurrentUnit)
            {
                GameObject baseObj = GameObject.Find("Canvas/TurnQueue/playerLI");
                GameObject li = Instantiate(baseObj);
                Transform trans = li.transform;
                trans.localPosition = new Vector3(130.0f, 150.0f);
                trans.localScale = new Vector3(0.6f, 0.6f);
                //GUI.contentColor = Color.blue;
                //GUI.Label(new Rect(initX, initY += offset, 100, 50), Instance.CurrentUnit.name);
                
            }
            else //All Other Units
            {
                GUI.contentColor = Color.black;
                GUI.Label(new Rect(initX, initY += offset, 100, 50), Instance.UnitQueue.ElementAt<Unit>(i).name);
            }
        }
    } */

    void Start () 
	{
        // Board = C
		StartTurn();
        MakeTurnQueue();
    }


    void UpdateTurnQueue()
    {
        GameObject baseObj = GameObject.Find("Canvas/TurnQueue/playerLI");
        GameObject parent = GameObject.Find("Canvas/TurnQueue");

        //Turn Queue Data
        for (int i = 0; i < Instance.UnitQueue.Count; i++)
        {   //Current Unit
            if (Instance.UnitQueue.ElementAt<Unit>(i) == Instance.CurrentUnit)
            {
                Transform elem = parent.transform;
                foreach(Transform child in elem)
                {
                    if(child.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text == Instance.CurrentUnit.name)
                    {
                        child.transform.GetChild(1).gameObject.SetActive(true);
                    }
                }

            }
            else //All Other Units
            {
                Transform elem = parent.transform;
                foreach (Transform child in elem)
                {
                    if (child.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text == Instance.UnitQueue.ElementAt<Unit>(i).name)
                    {
                        child.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void MakeTurnQueue(){
        //Create Turn Queue
        int offset = 0;
        float initX = 0.0f;
        float initY = 100.0f;
        GameObject baseObj = GameObject.Find("Canvas/TurnQueue/playerLI");
        GameObject parent = GameObject.Find("Canvas/TurnQueue");

        //Turn Queue Data
        for (int i = 0; i < Instance.UnitQueue.Count; i++)
        {   //Current Unit
            if (Instance.UnitQueue.ElementAt<Unit>(i) == Instance.CurrentUnit)
            {
                GameObject li = Instantiate(baseObj, parent.transform);
                Transform trans = li.transform;
                foreach (Transform child in trans)
                {
                    if (child.gameObject.tag == "Name")
                    {
                        child.gameObject.GetComponent<UnityEngine.UI.Text>().text = Instance.UnitQueue.ElementAt<Unit>(i).name;

                    }
                }
                trans.localPosition = new Vector3(initX, initY - offset);
                trans.localScale = new Vector3(0.6f, 0.6f);

            }
            else //All Other Units
            {
                GameObject li = Instantiate(baseObj, parent.transform);
                Transform trans = li.transform;
                foreach (Transform child in trans)
                {
                    if (child.gameObject.tag == "Name")
                    {
                        child.gameObject.GetComponent<UnityEngine.UI.Text>().text = Instance.UnitQueue.ElementAt<Unit>(i).name;
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                trans.localPosition = new Vector3(initX, initY - offset);
                trans.localScale = new Vector3(0.6f, 0.6f);
            }
            offset += 50;
        }
    }

    public void RemoveTurnQueue(){
        GameObject parent = GameObject.Find("Canvas/TurnQueue");
        Transform p = parent.transform;
        foreach (Transform child in p){
            if(child.gameObject.name == "playerLI(Clone)"){
                Destroy(child.gameObject);
            }
        }
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
		Instance.UnitQueue.OrderBy( u => u.Speed );
		// foreach(PlayerAction pa in Units[TurnQueue.Peek()])
		// {
		// 	pa.BeginTurn();
		// }
		foreach(Unit unit in Instance.UnitQueue)
		{
			unit.GetCurrentTile();
			unit.CurrentTile.Occupied = true;
            unit.AttackableTiles.Clear();
            unit.SelectableTiles.Clear();
		}
		if(Instance.UnitQueue.Count > 0)
		{
			Instance.CurrentUnit = Instance.UnitQueue.Peek();
			Instance.CurrentUnit.BeginTurn();
			if(CurrentUnit.isAI == true){
				AI.aiAction();
			}
		}


        UpdateTurnQueue();
    }

	public static string CheckWin()
	{
		List<string> NumberOfUnits = new List<string>();
		foreach(Unit unit in Instance.UnitQueue)
		{
			if(!NumberOfUnits.Contains(unit.tag.ToString()))
			{
				NumberOfUnits.Add(unit.tag.ToString());
			}
		}
		if(NumberOfUnits.Count == 1)
		{
			return NumberOfUnits.First();
		}
		return null;
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

        //if next unit is death, remove it from the queue
        if(Instance.UnitQueue.Peek().Health == 0){
            Instance.UnitQueue.Dequeue();
        }

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

    public void RemoveUnit(Unit unit)
    {
        if(Instance.Units.ContainsKey(unit.tag))
        {
            Instance.Units.Remove(unit.tag);
        }
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
