using UnityEngine;

public class MapPlayerTransform : MonoBehaviour
{
    public Transform transform;
    public Transform playerTransform;

    private void Awake()
    {
        if (transform == null)
        {
            Debug.Log("Player와 Map 간 연동 실패");
        }
    }

    void Start()
    {
        Player player = PlayerManager.Instance.Player;
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        FollowTransform();
    }

    private void FollowTransform()
    {
        transform.position = new Vector3
            (
            PlayerManager.Instance.Player.transform.position.x,
            100,
            PlayerManager.Instance.Player.transform.position.z
            );
    }
}
