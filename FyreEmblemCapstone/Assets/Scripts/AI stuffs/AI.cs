using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
	public static TurnManager TM;

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
	public static void aiAction(TurnManager tm){
		TM = tm;
		AI ai = new AI();

		//try to attack first
		int minHealth = 9999;
		TM.CurrentUnit.GetAttackableTiles();
		List<Tile> attackPos = TM.CurrentUnit.attackTiles;
		for(int x = 0; x < attackPos.Count;x++){
			foreach(var i in TM.UQ.ToArray()){
				i.GetCurrentTile();
				if(i.tag == "Player" && attackPos[x] == i.CurrentTile){
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
		
		// tm.CurrentUnit.FindSelectableTiles();
		// List<Tile> list = tm.CurrentUnit.SelectableTiles;
		// List<Tile> enemy = new List<Tile>();
		// for(int i = 0; i < list.Count; i++){
		// 	// if(!list[i].Selectable){
		// 	// 	enemy.Add(list[i]);  //add all tile have enemy list
		// 	// }
		// }

		// if(enemy.Count != 0){
		// 	tm.SelectAttack(); //attack
		// }else if(tm.CurrentUnit.Health < 5){
		// 	tm.SelectWait();
		// }else{
		// 	tm.SelectMove();
		// }
	}

	public int eval(Tile tile, Queue<Unit> unitQ){
		int totalScore = 0;
		//int ind = 0;
		bool[] result = new bool[2];

		// //Calculate damage and score of attack move
		// foreach (var i in unitQ.ToArray()){
		// 	//Unit taking damge from attack
		// 	if(i.CurrentTile == tile && i.tag == "Player"){
		// 		//i.Health -= TM.CurrentUnit.Attack; //implement later
		// 	}
		// 	if(i.Health <= 0){
		// 		unitQ.ToArray().ToList().RemoveAt(ind);   //remove unit from queue if health is zero
		// 	}
		// 		ind++;
		// }

		//Calculate the game over score
		result = gameOver(unitQ);
		if(result[0]){
			if(result[1] == true){
				totalScore += 9999;        //ai won
			}else{
				totalScore -= 9999;        //player won
			}
			return totalScore;
		}

		//Calculate total health score
		foreach (var i in unitQ.ToArray()){
			if(i.tag == "Enemy"){
				totalScore += i.Health;
			}else{
				totalScore -= i.Health;
				//Caculate distance score (if the unit health is low, run away as far as they can)
				if(TM.CurrentUnit.Health < 5){
					i.GetCurrentTile();
					totalScore += distance(i.CurrentTile,tile);
				}else{
					i.GetCurrentTile();
					//Debug.Log(i.CurrentTile);
					totalScore -= distance(i.CurrentTile,tile);
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
			if(i.tag == "Player"){
				enemyCount++;
			}
			if(i.tag == "Enemy"){
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
	int distance(Tile start, Tile end){
		int dis;
		
		float x1 = start.transform.position.x;
		float y1 = start.transform.parent.position.z;
		float x2 = end.transform.position.x;
		float y2 = end.transform.parent.position.z;

		dis = (int)Mathf.Sqrt(Mathf.Pow(Mathf.Abs(x1-x2),2)+Mathf.Pow(Mathf.Abs(y1-y2),2));

		return dis;	
	}
	// public int decisionAlgorithm(Tile tile, int score, Queue<Unit> unitQ){
	// 	int bestScore = score;
	// 	Queue<Unit> uq = unitQ;
		
	// 	if(isWon(uq)){
	// 		if(turn == 1){
	// 			bestScore += 9999;        //ai won
	// 		}else{
	// 			bestScore -= 9999;        //player won
	// 		}
	// 		return bestScore;
		
	// 	}else if(depth == 0){
	// 		bestScore = eval(tile, 0, uq);
	// 		return bestScore;        //end of the tree, return best score for the move
		
	// 	}else if(turn == 0){         //allied turn
	// 		List<Unit> units = uq.ToArray().ToList();
	// 		units[1].DisplayPossibleMoves();
	// 		List<Tile> list = units[1].SelectableTiles;
			
	// 		int ind = 0;  //keep track of indexes
	// 		foreach (var i in uq.ToArray()){
	// 			//Unit taking damge from attack
	// 			if(i.CurrentTile == tile && i.tag == "Player"){
	// 				i.Health -= TM.CurrentUnit.Attack;
	// 			}
	// 			if(i.Health <= 0){
	// 				uq.ToArray().ToList().RemoveAt(ind);   //remove unit from queue if health is zero
	// 			}
	// 			ind++;
	// 		}
			
	// 		Unit unit = uq.Dequeue();
	// 		uq.Enqueue(unit);
	// 		for(int i = 0; i < list.Count; i++){
	// 			int tempScore;
	// 			if(units[1].tag == "Enemy"){
	// 				tempScore = decisionTree(list[i], depth-1, 0, uq, 0);
	// 			}else{
	// 				tempScore = decisionTree(list[i], depth-1, 0, uq, 1);
	// 			}
				
	// 			if(tempScore > bestScore){
	// 				bestScore = tempScore;
	// 			}
	// 		}
	// 		return bestScore;
	// 	}else if(turn == 1){        //enemies turn
	// 		List<Unit> units = uq.ToArray().ToList();
		
	// 		//units[1].FindSelectableTiles();
	// 		List<Tile> list = units[1].SelectableTiles;
			
	// 		int ind = 0;  //keep track of indexes
	// 		foreach (var i in uq.ToArray()){
	// 			//Unit taking damge from attack
	// 			if(i.CurrentTile == tile && i.tag == "Enemy"){
	// 				i.Health -= TM.CurrentUnit.Attack;
	// 			}
	// 			if(i.Health <= 0){
	// 				uq.ToArray().ToList().RemoveAt(ind);   //remove unit from queue if health is zero
	// 			}
	// 			ind++;
	// 		}

	// 		Unit unit = uq.Dequeue();
	// 		uq.Enqueue(unit);
			
	// 		for(int i = 0; i < list.Count; i++){
	// 			int tempScore;
	// 			if(units[1].tag == "Enemy"){
	// 				tempScore = decisionTree(list[i], depth-1, 0, uq, 0);
	// 			}else{
	// 				tempScore = decisionTree(list[i], depth-1, 0, uq, 1);
	// 			}
	// 			if(tempScore < bestScore){
	// 				bestScore = tempScore;
	// 			}
	// 		}
	// 		return bestScore;
	// 	}else{
	// 		return bestScore;
	// 	}
	// }
	public Tile bestMove(List<Tile> tiles){
		Tile bestMv = null;
		int bestScore = -9999;
		List<Tile> equalScore = new List<Tile>();
		for(int i = 0; i < tiles.Count; i++){
			int tempScore = eval(tiles[i], TM.UQ);
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

		//Debug.Log(bestMv);
		return bestMv;	
	}
}
