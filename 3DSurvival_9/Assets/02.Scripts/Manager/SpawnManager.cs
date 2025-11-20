using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private GameObject playerPrefab;

    private GameObject _spawnedPlayerGameObject;
    private Transform _playerRootTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        _spawnedPlayerGameObject = Instantiate(playerPrefab, startPosition.position, Quaternion.identity);
        _playerRootTransform = _spawnedPlayerGameObject.transform.Find("Player");

        if (_playerRootTransform == null)
        {
            _playerRootTransform = _spawnedPlayerGameObject.transform;
        }
    }

    public void TeleportToSpawnPoint()
    {
        Rigidbody rb = _playerRootTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.MovePosition(startPosition.position);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            _playerRootTransform.position = startPosition.position;
        }
    }
}
