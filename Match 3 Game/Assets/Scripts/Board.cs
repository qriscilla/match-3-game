using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{

    public GameState currentState = GameState.move;
    public int width; // Each cell's width
    public int height; // Each cell's height
    public int offSet;
    public GameObject tilePrefab; // The tile we want to create
    public GameObject[] dots;
    private BackgroundTile[,] allTiles; // Creating an empty 2D array
    public GameObject[,] allDots; // Another 2D array
    private FindMatches findMatches;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
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
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject; // Instantiate(What you're instantiating, Position you're instantiating that, The rotation of the instantiated object)
                backgroundTile.transform.parent = this.transform; // Setting the parent to the board object
                backgroundTile.name = "( " + i + ", " + j + " )"; // Change name of backgroundTile
                int dotToUse = Random.Range(0, dots.Length); // Choose a random number between 0 and however long the dots array is

                int maxIterations = 0; // However many times it goes through the while loop below
                while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100) // Check if there any matches at the current location
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0; // Reset maxIterations back to zero

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;

                dot.transform.parent = this.transform; // The dot is now a child of the background tile
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if(allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        } else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if(allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row) // Helper method for destroying matches at a specific place
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched) // Check if dot is currently matched
        {
            findMatches.currentMatches.Remove(allDots[column, row]);
            Destroy(allDots[column, row]); // Destroy dots
            allDots[column, row] = null; // Make spot empty
        }
    }

    public void DestroyMatches() // Destroy all matches
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null) // Does a piece exist there?
                {
                    DestroyMatchesAt(i, j); // If so, destroy
                }
            }
        }
        StartCoroutine(DecreaseRowCo()); // Call DecreaseRowCo coroutine
    }

    private IEnumerator DecreaseRowCo() // A coroutine method
    {
        int nullCount = 0; // Count of empty spaces
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] == null) // If there's null spaces
                {
                    nullCount++; // Increase null count
                } else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0; // Reset nullCount
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard() // A helper method
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard()) // While MatchesOnBoard is true
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move; // Setting state to move
    }

}
