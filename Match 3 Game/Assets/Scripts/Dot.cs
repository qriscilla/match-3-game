using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{

    public int column; // x position
    public int row; // y position
    public int targetX;
    public int targetY;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition; // x-y position of the first touch
    private Vector2 finalTouchPosition; // x-y position of the final touch
    private Vector2 tempPosition;
    public float swipeAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x; // Cast targetX as an integer
        targetY = (int)transform.position.y; // Cast targetY as an integer
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column; // The main engine behind moving pieces around!
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        } else
        {
            // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
    }

    // Unity's built-in function that detects when user clicks on something with a collider
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(firstTouchPosition);
    }

    private void OnMouseUp() // When you're mouse is up (globally, not necessarily on the collider)
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle(); // Calling CalculateAngle() function
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        //Debug.Log(swipeAngle);
        MovePieces();
    }

    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width) // Takes into account the edge of the board
        {
            // Right swipe
            otherDot = board.allDots[column + 1, row]; // Find the dot to its right
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1; // Update the touched dot's column
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            // Up swipe
            otherDot = board.allDots[column, row + 1]; // Find the dot above
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1; // Update the touched dot's row
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) // OR because we're checking if it's greater than 135 or less than -135
        {
            // Left swipe
            otherDot = board.allDots[column - 1, row]; // Find the dot to its left
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1; // Update the touched dot's column
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down swipe
            otherDot = board.allDots[column, row - 1]; // Find the below dot
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1; // Update the touched dot's row
        }
    }

}
