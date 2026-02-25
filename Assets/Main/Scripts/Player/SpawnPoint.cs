using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    public string spawnName;

    void Start()
    {
        StartCoroutine(WaitForGameManager());
    }

    private IEnumerator WaitForGameManager()
    {
        // Wait until GameManager.Instance is set
        while (GameManager.Instance == null)
            yield return null;

        // Now safe to check spawn point
        if (GameManager.Instance.nextSpawnPoint == spawnName)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = transform.position;
            else
                Debug.LogError("[SpawnPoint] Player not found! Make sure it has tag 'Player'.");
        }
    }
}