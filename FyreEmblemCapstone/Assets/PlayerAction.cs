using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : PlayerMove 
{

	private enum SelectedAction{
		Attack,
		Wait,
		Move,
		Nothing
	}

	private SelectedAction CurrentAction = SelectedAction.Nothing;

	// Use this for initialization
	void Start () {
		MoveStart();
	}
	
	// Update is called once per frame
	void Update () {
		MoveUpdate();
	}
}
