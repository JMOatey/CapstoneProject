using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
	public static TurnManager TM = TurnManager.Instance;

	public static string PLAYER = TM.CurrentUnit.tag;
	public static string OPPONENT = PLAYER == "Enemy" ? OPPONENT = "Player" : OPPONENT = "Enemy";

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
		AI ai = new AI();

		//try to attack first
		int minHealth = 9999;
		TM.CurrentUnit.GetAttackableTiles();
		List<Tile> attackPos = TM.CurrentUnit.attackTiles;
		for(int x = 0; x < attackPos.Count;x++){
			foreach(var i in TM.UnitQueue.ToArray()){
				i.GetCurrentTile();
				if(i.tag == OPPONENT && attackPos[x] == i.CurrentTile){
					if(minHealth > i.Health){
						minHealth = i.Health;
						ATTACK = attackPos[x];
					}
				}
			}
		}
		//if no enemy to attack, don't attack
		if(minHealth != 9999 && TM.CurrentUnit.Health > 5){
			TM.SelectAttack();
		}
		
		TM.CurrentUnit.DisplayPossibleMoves();
		List<Tile> list = TM.CurrentUnit.SelectableTiles;
		MOVE = ai.bestMove(list);
		if(MOVE == null){
			TM.EndTurn();
		}else{
			TM.SelectMove();
		}
		
	}

	public float eval(Tile tile, Queue<Unit> unitQ){
		float totalScore = 0;
		bool[] result = new bool[2];

		//Calculate the game over score
		result = gameOver(unitQ);
		if(result[0]){
			if(result[1] == true){
				totalScore += 9999f;        //ai won
			}else{
				totalScore -= 9999f;        //player won
			}
			return totalScore;
		}

		//Calculate total health score
		foreach (var i in unitQ.ToArray()){
			if(i.tag == PLAYER){
				totalScore += i.Health;
			}else{
				totalScore -= i.Health;
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
			if(i.tag == OPPONENT){
				enemyCount++;
			}
			if(i.tag == PLAYER){
				playerCount++;
			}
		}

		if(playerCount == 0){
			result[0] = true;
			result[1] = false;
			return result;
		}

		if(enemyCount == 0){
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

	public Tile bestMove(List<Tile> tiles){
		Tile bestMv = null;
		float bestScore = -9999;
		List<Tile> equalScore = new List<Tile>();
		for(int i = 0; i < tiles.Count; i++){
			float tempScore = eval(tiles[i], TM.UnitQueue);
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
