using UnityEngine;
public class Tile : MonoBehaviour
{
    public int xCoord;
    public int yCoord;
    // public Unit unit;
    private GameObject Prefab;

    public Tile(int x, int y, GameObject tile)
    {
        this.xCoord = x;
        this.yCoord = y;
        this.Prefab = tile;
    }
}