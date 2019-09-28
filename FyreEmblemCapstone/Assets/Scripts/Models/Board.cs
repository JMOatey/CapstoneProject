using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Board : MonoBehaviour
{
    public int Width;
    public int Height;
    public List<Tile> Tiles;
    public Board(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.Tiles = new List<Tile>();
    }
}