using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
	public static TurnManager TM;

	//variable to keep current best move for current unit
	public static Tile MOVE;
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
		TM.CurrentUnit.FindSelectableTiles();
		List<Tile> list = tm.CurrentUnit.SelectableTiles;
		MOVE = ai.bestMove(list);

		List<Tile> enemyPos = new List<Tile>();
		foreach(var i in TM.UQ.ToArray()){
			if(i.tag == "Enemy" && MOVE == i.CurrentTile){
				tm.SelectAttack();
				return;
			}
		}
		
		if(MOVE == null){
			tm.SelectWait();
		}else{
			tm.SelectMove();
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

	public int eval(Tile tile, int score, Queue<Unit> unitQ){
		return 1;
	}

	public bool isWon(Queue<Unit> units){
		int unitCount = 0;
		int enemyCount = 0;

		foreach (var i in units.ToArray()){
			if(i.tag == "Player"){
				unitCount++;
			}
			if(i.tag == "Enemies"){
				enemyCount++;
			}
		}

		if(unitCount == 0 || enemyCount == 0){
			return true;
		}

		return true;
	}

	public int decisionTree(Tile tile, int depth, int score, Queue<Unit> unitQ, int turn){
		int bestScore = score;
		Queue<Unit> uq = unitQ;
		
		if(isWon(uq)){
			if(turn == 1){
				bestScore += 9999;        //ai won
			}else{
				bestScore -= 9999;        //player won
			}
			return bestScore;
		
		}else if(depth == 0){
			bestScore = eval(tile, bestScore, uq);
			return bestScore;        //end of the tree, return best score for the move
		
		}else if(turn == 0){         //allied turn
			uq.ToArray().ToList().ElementAt(1).FindSelectableTiles();
			List<Tile> list = uq.ToArray().ToList().ElementAt(1).SelectableTiles;
			
			int ind = 0;  //keep track of indexes
			foreach (var i in uq.ToArray()){
				//Unit taking damge from attack
				if(i.CurrentTile == tile && i.tag == "Player"){
					i.Health -= TM.CurrentUnit.Attack;
				}
				if(i.Health <= 0){
					uq.ToArray().ToList().RemoveAt(ind);   //remove unit from queue if health is zero
				}
				ind++;
			}
			
			Unit unit = uq.Dequeue();
			uq.Enqueue(unit);

			for(int i = 0; i < list.Count; i++){
				int tempScore;
				if(uq.ToArray().ToList().ElementAt(1).tag == "Enemy"){
					tempScore = decisionTree(list[i], depth-1, 0, uq, 0);
				}else{
					tempScore = decisionTree(list[i], depth-1, 0, uq, 1);
				}
				
				if(tempScore > bestScore){
					bestScore = tempScore;
				}
			}
		
		}else if(turn == 1){        //enemies turn
			uq.ToArray().ToList().ElementAt(1).FindSelectableTiles();
			List<Tile> list = uq.ToArray().ToList().ElementAt(1).SelectableTiles;
			
			int ind = 0;  //keep track of indexes
			foreach (var i in uq.ToArray()){
				//Unit taking damge from attack
				if(i.CurrentTile == tile && i.tag == "Enemy"){
					i.Health -= TM.CurrentUnit.Attack;
				}
				if(i.Health <= 0){
					uq.ToArray().ToList().RemoveAt(ind);   //remove unit from queue if health is zero
				}
				ind++;
			}

			Unit unit = uq.Dequeue();
			uq.Enqueue(unit);
			
			for(int i = 0; i < list.Count; i++){
				int tempScore;
				if(uq.ToArray().ToList().ElementAt(1).tag == "Enemy"){
					tempScore = decisionTree(list[i], depth-1, 0, uq, 0);
				}else{
					tempScore = decisionTree(list[i], depth-1, 0, uq, 1);
				}
				if(tempScore < bestScore){
					bestScore = tempScore;
				}
			}
		}

		return bestScore;
	}
	public Tile bestMove(List<Tile> tiles){
		Tile bestMv = null;
		int bestScore = -1;
		for(int i = 0; i < tiles.Count; i++){
			int tempScore = decisionTree(tiles[i], 2, 0, TM.UQ, 0);
			if(tempScore > bestScore){
				bestScore = tempScore;
				bestMv = tiles[i];
			}

		}
		return bestMv;
	}
}
