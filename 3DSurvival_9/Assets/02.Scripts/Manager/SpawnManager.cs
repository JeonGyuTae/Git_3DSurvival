using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private GameObject playerPrefab;

    private Player player;

    private void Awake()
    {
        player = PlayerManager.Instance.Player;

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab);
        player.transform.position = startPosition.position;
    }

    public void TeleportToSpawnPoint()
    {
        player.transform.position = startPosition.position;
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
