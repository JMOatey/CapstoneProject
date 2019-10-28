using UnityEngine;

public class PlayerUtility : MonoBehaviour 
{
	public bool Turn = false;
    public GameObject[] Tiles;
	public Tile CurrentTile; 
	public List<Tile> SelectableTiles = new List<Tile>();

    public void GetCurrentTile()
	{
		CurrentTile = GetTargetTile(gameObject);
		CurrentTile.Current = true;
	}

}