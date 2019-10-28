using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : TurnManager
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    	//AI choose action base on the current situation
	public static void aiAction(TurnManager tm){
		tm.CurrentUnit.DisplayPossibleMoves();
		List<Tile> list = tm.CurrentUnit.SelectableTiles;
		List<Tile> enemy = new List<Tile>();
		for(int i = 0; i < list.Count; i++){
			// if(list[i].Selectable){
			// 	enemy.Add(list[i]);  //add all tile have enemy list
			// }
			if(!list[i].Selectable){
				enemy.Add(list[i]);  //add all tile have enemy list
			}
		}

		if(enemy.Count != 0){
			tm.SelectAttack(); //attack
		}else if(tm.CurrentUnit.Health < 5){
			tm.SelectWait();
		}else{
			tm.SelectMove();
		}
	}
}
