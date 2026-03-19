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

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);

        // La rotación inicial se establece en el Spawner.
        if (player != null)
        {
            float distanceForward = Vector3.Dot(transform.position - player.position, player.forward);
            if (distanceForward < -30f)
            {
                Destroy(gameObject);
            }
        }
    }
}
