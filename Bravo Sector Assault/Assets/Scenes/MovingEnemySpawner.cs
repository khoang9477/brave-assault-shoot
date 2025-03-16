using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemySpawner : MonoBehaviour
{
    //spawner behavior
    public enum SpawnerType {Left, Right, Top}

    public SpawnerType spawnerType;
    public float spawnerSpeed = 2.5f;
    public float movementRange = 4f;

    private Vector3 startPosition;
    private float timeElapsed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; //initialized position
    }

    // Update is called once per frame
    void Update()
    {
        MoveSpawner();
    }

    void MoveSpawner()
    {
        timeElapsed += Time.deltaTime;

        //move enemy spawner depends on the direction
        if (spawnerType == SpawnerType.Left || spawnerType == SpawnerType.Right)
        {
            float spawnerYMove = Mathf.PingPong(timeElapsed, movementRange);
            if (spawnerType == SpawnerType.Left)
            {
                transform.position = new Vector3(startPosition.x, startPosition.y + spawnerYMove, startPosition.z);
            }
            else if (spawnerType == SpawnerType.Right)
            {
                transform.position = new Vector3(startPosition.x, startPosition.y - spawnerYMove, startPosition.z);
            }
        }
        else if (spawnerType == SpawnerType.Top)
        {
            float spawnerXMove = Mathf.PingPong(timeElapsed, movementRange);
            transform.position = new Vector3(startPosition.x + spawnerXMove, startPosition.y, startPosition.z);
        }
    }
}
