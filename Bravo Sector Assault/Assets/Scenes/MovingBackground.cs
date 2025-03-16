using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    //background behavior
    public GameObject[] levels;
    private Camera mainCamera;
    private Vector2 screenBounds;
    public float choke; //some piece of chunk left on border (only edit on Unity if background has issues)
    public float[] scrollSpeed; //background speed

    // Get component for camera to background
    void Start()
    {  
       mainCamera = gameObject.GetComponent<Camera>(); //define camera
       screenBounds = mainCamera.ScreenToWorldPoint(new Vector3 (Screen.width, Screen.height, mainCamera.transform.position.z)); //define screen bounds
        if (scrollSpeed.Length != levels.Length)
        {
            Debug.LogError("Scroll speed array size does not match levels array size!");
            return;
        }
        
        foreach (GameObject obj in levels) //creating reference to each levels
        {
            loadEachChildObjects(obj);
        }
    }

    //Load each child on background including main background
    void loadEachChildObjects(GameObject obj)
    {
        float objectWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x - choke; //only for horizontal scrolling
        float objectHeight = obj.GetComponent<SpriteRenderer>().bounds.size.y - choke; //only for vertical scrolling

        //Only need y
        int childsNeed = (int)Mathf.Ceil(screenBounds.y * 2 / objectHeight); //calculate child needs
        //clone the object
        GameObject clone = Instantiate(obj) as GameObject;

        //loop each child need to clone
        for (int i = 0; i <= childsNeed; i++)
        {
            GameObject cloneChild = Instantiate(clone) as GameObject;
            cloneChild.transform.SetParent(obj.transform); //set object to parent object
            //space out in y and/or x depends on game
            cloneChild.transform.position = new Vector3(obj.transform.position.x, objectHeight * i, obj.transform.position.z);
            cloneChild.name = obj.name + i; //log child name
        }
        Destroy(clone); //finish loop
        Destroy(obj.GetComponent<SpriteRenderer>()); //no longer need sprite render after loop
    }
    void repositionEachChildObjects(GameObject obj)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>(); //transform all child objects
        //check for error
        if (children.Length > 1)
        {
            GameObject firstChild = children[1].gameObject; //first child (first background part)
            GameObject lastChild = children[children.Length - 1].gameObject; //last child (last background part)

            //calculate half the screen
            float halfObjectHeight = lastChild.GetComponent<SpriteRenderer>().bounds.extents.y - choke;
            //NOTE: 0 is parent, not child. Negative value is not accepted.

            //two if statement here is for when reaching edge of the screen (top screen)
            if (transform.position.y + screenBounds.y > lastChild.transform.position.y + halfObjectHeight)
            {
                firstChild.transform.SetAsLastSibling(); //set the top screen to the bottom screen
                //move the camera to bottom screen
                firstChild.transform.position = new Vector3(lastChild.transform.position.x, lastChild.transform.position.y + halfObjectHeight * 2, lastChild.transform.position.z);
            }
            //second statement is reverse (bottom screen)
            else if(transform.position.y - screenBounds.y < lastChild.transform.position.y - halfObjectHeight)
            {
                lastChild.transform.SetAsFirstSibling(); //set the bottom screen to the top screen
                //move the camera to top screen
                lastChild.transform.position = new Vector3(firstChild.transform.position.x, firstChild.transform.position.y - halfObjectHeight * 2, firstChild.transform.position.z);
            }

        }
    }

    void LateUpdate() //call after update function is called
    {
        foreach(GameObject obj in levels)
        {
            //reposition child objects
            repositionEachChildObjects(obj);
        }
    }

    void Update()
    {
       for (int i = 0; i < levels.Length; i++)
        {
            GameObject obj = levels[i];
            obj.transform.position += Vector3.up * scrollSpeed[i] * Time.deltaTime;
            repositionEachChildObjects(obj);
        }
    }
}
