using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public int width; // Each cell's width
    public int height; // Each cell's height
    public GameObject tilePrefab; // The tile we want to create
    private BackgroundTile[,] allTiles; // Creating an empty 2D array

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        SetUp(); // Calling the SetUp() method
    }

    private void SetUp()
    {
        for(int i = 0; i < width; i ++) // i represents the x-coordinate
        {
            for(int j = 0; j < height; j ++) // j represents the y-coordinate
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject; // Instantiate(What you're instantiating, Position you're instantiating that, The rotation of the instantiated object)
                backgroundTile.transform.parent = this.transform; // Setting the parent to the board object
                backgroundTile.name = "( " + i + ", " + j + " )"; // Change name of backgroundTile
            }
        }
    }

}
