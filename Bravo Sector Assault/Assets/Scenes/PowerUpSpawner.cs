using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUps; //Assign different power-up
    public float spawnRate = 20f; //spawn in 20 seconds
    

    void Start()
    {
        InvokeRepeating(nameof(SpawnPowerUp), 2f, spawnRate);
    }

    void SpawnPowerUp()
    {
        Vector2 randomPos = new Vector2(Random.Range(-3.5f, 3.5f), Random.Range(-3.5f,3.5f));
        Instantiate(powerUps[Random.Range(0, powerUps.Length)], randomPos, Quaternion.identity);
    }
}
