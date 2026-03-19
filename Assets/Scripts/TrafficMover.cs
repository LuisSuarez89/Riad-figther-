using UnityEngine;

public class TrafficMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private bool faceMovementDirection = true;

    private Transform player;

    public void Initialize(Transform targetPlayer, float speed)
    {
        player = targetPlayer;
        moveSpeed = speed;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            return;
        }

        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        if (faceMovementDirection)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * 6f);
        }

        if (player != null && transform.position.z < player.position.z - 30f)
        {
            Destroy(gameObject);
        }
    }
}
