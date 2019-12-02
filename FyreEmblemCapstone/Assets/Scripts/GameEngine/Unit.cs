using System.Collections;
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
	public string side;   //keep track of which side unit is on, for temporary unit
	public bool Finished = false;
	public float Speed = 0;
	public int Health = 10;
    public GameObject WinGraphic;
    public GameObject LoseGraphic;

	public SelectedAction CurrentAction = SelectedAction.Nothing;

	void Start () {
		this.HasMoved = false;
		TurnManager.Instance.AddUnit(this);

		MoveStart();
		// GetCurrentTile();

		GetCurrentTile();
		SelectableTiles.Clear();
		AttackableTiles.Clear();
		SelectableTiles.FindAvailableTiles(MoveDistance, CurrentTile, JumpHeight, Tiles);
		GetAttackableTiles();
        HasMoved = false;
		Turn = true;
		HasAttacked = false;
		// ShowEveryOption();
		// HideEverything();
	}
	
	void Update () {
		// Debug.Log(transform.position);
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

        //Update Health Bar
        Transform bar = transform.Find("HP/HealthBar");
        bar.localScale = new Vector3(((float)Health / 10.0f), 0.1f, 1.0f);

		IEnumerator die(){
			yield return new WaitForSeconds(2);
			Destroy(this.gameObject);
		}

        IEnumerator playEndMusic(string win)
        {

            yield return new WaitForSeconds(6);
        }

        if (Health == 0)
        {
            //Death Animation
            Transform model = transform.Find("Player Model");
            Animator anim = model.GetComponent<Animator>();
            anim.Play("death", -1);

            //Remove character from Unit Queue if dead
            TurnManager.Instance.UnitQueue = new Queue<Unit>(TurnManager.Instance.UnitQueue.Where(s => s != this));
            TurnManager.Instance.RemoveUnit(this);
			//Clear the turn queue UI
			TurnManager.Instance.RemoveTurnQueue();
			//Re-make the queue UI
			TurnManager.Instance.MakeTurnQueue();
			//Destroy unit game object
			StartCoroutine(die());

            //Check Win conditions
            string win = TurnManager.CheckWin();
            if (win != null)
            {
                //stop background music
				Debug.Log("here");
                GameObject bgMusic = GameObject.Find("BackgroundMusic");
                Transform t = bgMusic.transform;
                AudioSource s = t.GetComponent<AudioSource>();
                s.Stop();

                //UI Stuff for win/loss condition
                //if player lose
                if (win != tag && this.isAI == false)
                {
                    //play loss music
                    Debug.Log("loss");
                    GameObject lossmusic = GameObject.Find("LossMusic");
                    Transform trans = lossmusic.transform;
                    AudioSource lossSound = trans.GetComponent<AudioSource>();
                    lossSound.Play();

                    //Display Lose Graphic
                    GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>();
                    foreach(GameObject obj in objs)
                    {
                        if(obj.name == "LoseGraphic")
                        {
                            LoseGraphic = obj;
                            LoseGraphic.SetActive(true);
                        }
                    }
                }
                //if Ai lose
                else if (win != tag && this.isAI == true)
                {
                    //Play win music
                    Debug.Log("win");
                    GameObject winmusic = GameObject.Find("WinMusic");
                    Transform ts = winmusic.transform;
                    AudioSource winSound = ts.GetComponent<AudioSource>();
                    winSound.Play();

                    //Display Win Graphic
                    GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>();
                    foreach (GameObject obj in objs)
                    {
                        if (obj.name == "WinGraphic")
                        {
                            WinGraphic = obj;
                            WinGraphic.SetActive(true);
                        }
                    }
                }
            }

            Health = -1;
        }
    }

	protected void MoveUpdate ()
	{
		if(!Turn)
		{
			return;
		}
		if(HasMoved){
			if(this.isAI == true){
				TurnManager.Instance.EndTurn();
			}
			return;
		}
		if(!Moving)
		{
			if(this.isAI == true){
				aiMove(AI.MOVE);
			}else{
				DisplayPossibleMoves();
				CheckMouse();
			}
		}
		else
		{
            //Play move animation and move
            Transform model = transform.Find("Player Model");
            Animator anim = model.GetComponent<Animator>();
            anim.Play("walk", -1);
			Move();
		}
	}

	//Move to random tile
	void aiMove(Tile move){
		// List<Tile> list = this.SelectableTiles;
		// MoveToTile(list[Random.Range(0,list.Count)]);
		MoveToTile(move);
	}
	void aiAttack(Tile attack){
		foreach (var i in TurnManager.Instance.UnitQueue.ToArray()){
			i.GetCurrentTile();
			if(i.tag == AI.OPPONENT && i.CurrentTile == attack){
				//Play Attack Animation
                Animator anim = TurnManager.Instance.CurrentUnit.GetComponentInChildren<Animator>();
                anim.Play("attack", -1);

				if(i.Health > 0){
					//onHit Animation
                    Animator hitAnimation = i.GetComponentInChildren<Animator>();
                    hitAnimation.Play("onHit", -1);
				}
				
				i.Health -= AttackDamage;
				HasAttacked = true;
			}
		}
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
		GetSelectableTiles();
		GetAttackableTiles();
        HasMoved = false;
		Turn = true;
		HasAttacked = false;

        Transform indicator = transform.Find("Indicator/Cylinder");
        Transform effect = transform.Find("Indicator/Cylinder/Sparkles");
        indicator.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        effect.localScale = new Vector3(0.3f, 0.3f, 0.3f);
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

		HasMoved = false;
        Transform indicator = transform.Find("Indicator/Cylinder");
        Transform effect = transform.Find("Indicator/Cylinder/Sparkles");
        indicator.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        effect.localScale = new Vector3(0.0f, 0.0f, 0.0f);
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
		foreach(Unit unit in TurnManager.Instance.UnitQueue)
		{
			unit.GetCurrentTile();
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
			if(this.isAI == true){
				if(AI.ATTACK != null){
					aiAttack(AI.ATTACK);
				}else{
					HasAttacked = true;
				}
			}else{
				Attack();
			}	
		}
		GetAttackableTiles();
		DisplayAttackableTiles();
		if(HasAttacked && this.isAI == true){
			TurnManager.Instance.EndTurn();
		}
    }

	void Attack()
	{
		if(Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			string team = TurnManager.Instance.CurrentUnit.tag;

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				Unit unit = hit.collider.GetComponent<Unit>();
				if(AttackableTiles.Contains(unit.CurrentTile) && ((hit.collider.tag == "Player" && team == "Enemy") || (hit.collider.tag == "Enemy" && team == "Player")))
				{
                    //Play Attack Animation
                    Transform model = transform.Find("Player Model");
                    Animator anim = model.GetComponent<Animator>();
                    anim.Play("attack", -1);

                    //Attack Logic
					Debug.Log(unit);
					Debug.Log(unit.Health);
                    if(unit.Health > 0)
                    {
                        //onHit Animation
                        Animator hitAnimation = hit.collider.GetComponentInChildren<Animator>();
                        hitAnimation.Play("onHit", -1);

                        unit.Health -= AttackDamage;
                        if(unit.Health < 0)
                        {
                            unit.Health = 0;
                        }
                    }
					HasAttacked = true;
				}
			}
		}
	}

	public void GetSelectableTiles()
	{
		foreach(GameObject tile in Tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindNeighbors(JumpHeight);
		}
		Queue<Tile> process = new Queue<Tile>();

		process.Enqueue(CurrentTile);
		CurrentTile.Visited = true;

		while(process.Count > 0)
		{
			Tile t = process.Dequeue();
			
			SelectableTiles.Add(t);
			if(t.Distance < (MoveDistance))
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

    public void GetAttackableTiles()
    {
		foreach(GameObject tile in Tiles)
		{
			Tile t = tile.GetComponent<Tile>();
			t.FindAttackNeighbors(JumpHeight);
		}
		if(!HasMoved && CurrentAction != SelectedAction.Attack)
		{
			if(AttackRange <= 1)
			{
				foreach(Tile tile in SelectableTiles)
				{
					tile.Reset();
					tile.FindAttackNeighbors(JumpHeight);
					AttackableTiles.AddRange(tile.AdjacencyList);
				}
			}
			// Queue<Tile> process = new Queue<Tile>();

			// process.Enqueue(CurrentTile);
			// CurrentTile.Visited = true;

			// while(process.Count > 0)
			// {
			// 	Tile t = process.Dequeue();
				
			// 	AttackableTiles.Add(t);
			// 	if(t.Distance < (MoveDistance + AttackRange))
			// 	{
			// 		foreach(Tile tile in t.AdjacencyList)
			// 		{
			// 			if(!tile.Visited)
			// 			{
			// 				tile.Parent = t;
			// 				tile.Visited = true;
			// 				tile.Distance = 1 + t.Distance;
			// 				process.Enqueue(tile);
			// 			}
			// 		}
			// 	}
			// }

		}
		else
		{
			Queue<Tile> process = new Queue<Tile>();

			process.Enqueue(CurrentTile);
			CurrentTile.Visited = true;

			while(process.Count > 0)
			{
				Tile t = process.Dequeue();
				
				AttackableTiles.Add(t);
				if(t.Distance < AttackRange)
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