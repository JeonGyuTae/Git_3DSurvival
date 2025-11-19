using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
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
}
