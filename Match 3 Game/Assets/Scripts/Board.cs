using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public int width; // Each cell's width
    public int height; // Each cell's height
    public GameObject tilePrefab; // The tile we want to create
    public GameObject[] dots;
    private BackgroundTile[,] allTiles; // Creating an empty 2D array
    public GameObject[,] allDots; // Another 2D array

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
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
                int dotToUse = Random.Range(0, dots.Length); // Choose a random number between 0 and however long the dots array is
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform; // The dot is now a child of the background tile
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

}
