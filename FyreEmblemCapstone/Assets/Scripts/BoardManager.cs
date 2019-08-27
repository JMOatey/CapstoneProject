using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour 
{
	public int Width;
	public int Height;
	public GameObject Cube1;
	public GameObject Cube2;

	// Use this for initialization
	void Start()
	{
		CreateBoard();
	}
	
	private void CreateCube(GameObject cube, int posX, int posY, int posZ)
	{
		GameObject.Instantiate(cube, new Vector3(posX, posY, posZ), Quaternion.identity);
	}

	private void CreateBoard()
	{
		bool isWhite = true;
		for(int i = 0; i < this.Width; i++)
		{
			for(int j = 0; j < this.Height; j++)
			{
				if(isWhite)
				{
					CreateCube(Cube1, i, 0, j);
				} else 
				{
					CreateCube(Cube2, i, 0, j);
				}
				isWhite = !isWhite;
			}
			isWhite = !isWhite;
		}
	}
}
