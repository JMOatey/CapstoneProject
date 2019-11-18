using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
	public static TurnManager TM;

	//keep track of current AI
	public static string PLAYER;

	//keep track of current AI's opponent
	public static string OPPONENT;

	//variable to keep current best move for current unit
	public static Tile MOVE;

	//variable to keep the current tile for attack
	public static Tile ATTACK;

    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {

    }
	//Reference: https://www.geeksforgeeks.org/minimax-algorithm-in-game-theory-set-3-tic-tac-toe-ai-finding-optimal-move/
    //AI choose action base on the current situation
	public static void aiAction(){
		//initialize the variables
		TM = TurnManager.Instance;
		PLAYER = TM.CurrentUnit.tag;
		OPPONENT = PLAYER == "Enemy" ? OPPONENT = "Player" : OPPONENT = "Enemy";
		
		AI ai = new AI();

		//try to attack first
		ATTACK = ai.bestMove(TM.CurrentUnit.attackTiles, true);
		if(ATTACK != null){
			TM.SelectAttack();
		//if not attack this turn, then move or wait
		}else{
			TM.CurrentUnit.DisplayPossibleMoves();
			List<Tile> list = TM.CurrentUnit.SelectableTiles;
			MOVE = ai.bestMove(list, false);
			if(MOVE == null){
				TM.EndTurn();
			}else{
				TM.SelectMove();
			}
		}
		
	}

	public float eval(Tile tile, Queue<Unit> unitQ, bool isAttack){
		float totalScore = 0;
		bool[] result = new bool[2];
		//make a copy of the unit queue
		Queue<Unit> UQ = new Queue<Unit>();
		bool possAttack = false;      //keep track of whether an attack is possible

		//calculate the attack score
		if(isAttack){
			int ex = (int)tile.transform.position.x;
			int ey = (int)tile.transform.parent.position.z;
			int px = (int)TM.CurrentUnit.CurrentTile.transform.position.x;
			int py = (int)TM.CurrentUnit.CurrentTile.transform.parent.position.z;
			int range = TM.CurrentUnit.AttackRange;

			foreach (var i in unitQ.ToArray()){
				//make a copy of the unit
				Unit temp = new Unit();
				temp.Health = i.Health;
				temp.side = i.tag;
				if(i.CurrentTile == tile && i.tag == OPPONENT && ((ex <= px+range && ex >= px-range && ey == py) ^ (ey <= py+range && ey >= py-range && ex == px))){
					if(TM.CurrentUnit.Health > 5){
						temp.Health -= TM.CurrentUnit.AttackDamage;
						totalScore -= temp.Health;
						possAttack = true;
					}
				}
				UQ.Enqueue(temp);
			}

			//Calculate the game over score
			result = gameOver(UQ);
			if(result[0]){
				if(result[1] == true){
					totalScore += 9999;        //ai won
				}else{
					totalScore -= 9999;        //player won
				}
				return totalScore;
			}
			
			if(possAttack){
				return totalScore;
			}else{
				return -10000;
			}
		}

		//Calculate the moving score
		foreach (var i in unitQ.ToArray()){
			if(i.tag == PLAYER){
				//totalScore += i.Health;
			}else{
				//totalScore -= i.Health;
				//Caculate distance score (if the unit health is lower than attack target, put as much distance between them and their enemy as they can)
				if(TM.CurrentUnit.Health < i.Health){
					i.GetCurrentTile();
					totalScore += Mathf.Sqrt(distance(i.CurrentTile,tile));
				}else{
					i.GetCurrentTile();
					totalScore -= Mathf.Sqrt(distance(i.CurrentTile,tile));
				}
			}
		}
		
		
		return totalScore;
	}

	//return  whether the game is over and AI win or lose
	private bool[] gameOver(Queue<Unit> units){
		int playerCount = 0;
		int enemyCount = 0;
		bool[] result = new bool[2];

		foreach (var i in units.ToArray()){
			if(i.side == OPPONENT && i.Health > 0){
				enemyCount++;
			}
			if(i.side == PLAYER && i.Health > 0){
				playerCount++;
			}
		}

		if(playerCount == 0){       //ai win
			result[0] = true;
			result[1] = false;
			return result;
		}

		if(enemyCount == 0){         //ai lose
			result[0] = true;
			result[1] = true;
			return result;
		}

		result[0] = false;
		result[1] = false;
		return result;
	}

	//calcualte the distance between 2 tiles
	float distance(Tile start, Tile end){
		float dis;
		
		float x1 = start.transform.position.x;
		float y1 = start.transform.parent.position.z;
		float x2 = end.transform.position.x;
		float y2 = end.transform.parent.position.z;

		dis = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(x1-x2),2)+Mathf.Pow(Mathf.Abs(y1-y2),2));

		return dis;	
	}

	public Tile bestMove(List<Tile> tiles, bool isAttack){
		Tile bestMv = null;
		float bestScore = -9999;
		List<Tile> equalScore = new List<Tile>();

		//return the best attack move, null if it is not possible to attack
		if(isAttack){
			foreach(var i in TM.UnitQueue.ToArray()){
				i.GetCurrentTile();
				//find unit within attack range
				if(i.tag == OPPONENT){
					float tempScore = eval(i.CurrentTile, TM.UnitQueue, isAttack);
				
					if(tempScore > bestScore){          //for move with equal values, get the lastest one
						bestScore = tempScore;
						equalScore.Clear();
						equalScore.Add(i.CurrentTile);
						bestMv = i.CurrentTile;
					}
					if(tempScore == bestScore){
						equalScore.Add(i.CurrentTile);
					}
				}
			}
			
			if(equalScore.Count != 0){
				bestMv = equalScore[Random.Range(0,equalScore.Count)];
			}
			return bestMv;	
		}

		for(int i = 0; i < tiles.Count; i++){
			float tempScore = eval(tiles[i], TM.UnitQueue, isAttack);
			//Debug.Log(tempScore);
			if(tempScore > bestScore){          //for move with equal values, get the lastest one
				bestScore = tempScore;
				equalScore.Clear();
				equalScore.Add(tiles[i]);
				bestMv = tiles[i];
			}
			if(tempScore == bestScore){
				equalScore.Add(tiles[i]);
			}
		}

		bestMv = equalScore[Random.Range(0,equalScore.Count)];

		return bestMv;	
	}
}
