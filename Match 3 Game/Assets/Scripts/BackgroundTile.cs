using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    public GameObject[] dots;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Calling the Initialize() method 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Initialize()
    {
        int dotToUse = Random.Range(0, dots.Length); // Choose a random number between 0 and however long the dots array is
        GameObject dot = Instantiate(dots[dotToUse], transform.position, Quaternion.identity);
        dot.transform.parent = this.transform; // The dot is now a child of the background tile
        dot.name = this.gameObject.name;
    }
}
