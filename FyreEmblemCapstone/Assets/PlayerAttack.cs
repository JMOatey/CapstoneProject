using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerUtility
{
    public int Attack = 2;
	public int AttackRange = 1;
    List<Tile> ReachableTiles = new List<Tile>();

    // Start is called before the first frame update
    void AttackStart()
    {
        
    }

    // Update is called once per frame
    void AttackUpdate()
    {
        
    }

    void GetReachableTiles()
    {
        // GameObject.Find
    }
}
