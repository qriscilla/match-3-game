using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{

    [Header("Board Variables")]
    public int column; // x position
    public int row; // y position
    public int previousColumn; // Variable for keeping track of previous column
    public int previousRow; // Variable for keeping track of previous row
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition; // x-y position of the first touch
    private Vector2 finalTouchPosition; // x-y position of the final touch
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f; // If movement is not "far" enough or a unit or more in length, it won't swipe

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        //FindMatches();
        if(isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f); // Change to white with transparency of 0.2
        }
        targetX = column; // The main engine behind moving pieces around!
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if(board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        } else
        {
            // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo() // Coroutines run parallel to another method. They require a return statement.
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) // If current dot and the other dot aren't matching
            {
                // Move them back to where they were
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            } else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        }
    }

    // Unity's built-in function that detects when user clicks on something with a collider
    private void OnMouseDown()
    {
        if(board.currentState == GameState.move) // If game is in move state
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp() // When you're mouse is up (globally, not necessarily on the collider)
    {
        if(board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle(); // Calling CalculateAngle() function
        }
    }

    void CalculateAngle()
    {
        // Only calculate angle if movement goes over threshold of swipeResist
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            //board.currentState = GameState.wait; // Change game state to wait as soon as we calculate the angle
        } else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) // Takes into account the edge of the board
        {
            // Right swipe
            otherDot = board.allDots[column + 1, row]; // Find the dot to its right
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1; // Update the touched dot's column
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up swipe
            otherDot = board.allDots[column, row + 1]; // Find the dot above
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1; // Update the touched dot's row
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) // OR because we're checking if it's greater than 135 or less than -135
        {
            // Left swipe
            otherDot = board.allDots[column - 1, row]; // Find the dot to its left
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1; // Update the touched dot's column
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down swipe
            otherDot = board.allDots[column, row - 1]; // Find the below dot
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1; // Update the touched dot's row
        }
        StartCoroutine(CheckMoveCo());
    }

    void FindMatches()
    {
        // For checking left and right
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row]; // For the dot directly to the left
            GameObject rightDot1 = board.allDots[column + 1, row]; // For the dot directly to the right
            if(leftDot1 != null && rightDot1 != null)
            {
                if(leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        // For checking up and down
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1]; // For the dot directly above
            GameObject downDot1 = board.allDots[column, row - 1]; // For the dot directly below
            if(upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

}
