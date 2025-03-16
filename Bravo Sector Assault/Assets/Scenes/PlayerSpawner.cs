using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    //attach player prefab for player spawn
    public GameObject playerPrefab;

    //network spawn
    void Start()
    {
        if (!IsServer) return; // Only the server spawns players

        Debug.Log("Server started, waiting for players...");
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    void SpawnPlayer(ulong clientId) //spawn player
    {
        Debug.Log($"Spawning player for client {clientId}");

        // check prefab exists
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in PlayerSpawner!");
            return;
        }

        GameObject player = Instantiate(playerPrefab, GetSpawnPosition(clientId), Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetSpawnPosition(ulong clientId)
    {
        // spawn position
        Vector3 spawnPosition = clientId % 2 == 0 ? new Vector3(-1, -4, 0) : new Vector3(1, -4, 0);
        Debug.Log($"Client {clientId} spawned at {spawnPosition}");
        return spawnPosition;
    }

    void OnPlayerJoined(ulong clientId)
    {
         Debug.Log("Player joined with ID: " + clientId);
    }

    public override void OnDestroy()   //remove callback in case of memory leaks
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
}
