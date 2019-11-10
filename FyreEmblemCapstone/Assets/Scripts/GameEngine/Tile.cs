using System.Collections.Generic;
using UnityEngine;
public class Tile : MonoBehaviour
{
    public bool Walkable = true;
    public bool Occupied = false;
    public bool Target = false;
    public bool Selectable = false;
    public bool Attackable = false;

    public List<Tile> AdjacencyList = new List<Tile>();

    public bool Visited = false;
    public Tile Parent = null; 
    public int Distance = 0;

    void Update() {
        if(Occupied)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if(Target)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if(Selectable)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if(Attackable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void Reset()
    {
        Walkable = true;
        Occupied = false;
        Target = false;
        Selectable = false;
        Attackable = false;

        AdjacencyList.Clear();

        Visited = false;
        Parent = null; 
        Distance = 0;
    }

    public void FindNeighbors(float jumpHeight)
    {
        Reset();

        CheckTile(Vector3.forward, jumpHeight);
        CheckTile(-Vector3.forward, jumpHeight);
        CheckTile(Vector3.right, jumpHeight);
        CheckTile(-Vector3.right, jumpHeight);
    }

    public void CheckTile(Vector3 direction, float jumpHeight)
    {
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach(Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if(tile != null && tile.Walkable)
            {
                RaycastHit hit;

                if(!Physics.Raycast(tile.transform.position, Vector2.up, out hit, 1))
                {
                    AdjacencyList.Add(tile);
                }
            }
        }
    }
}