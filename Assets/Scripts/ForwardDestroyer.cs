using UnityEngine;

public class ForwardDestroyer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float destroyBehindDistance = 25f;

    public void SetPlayer(Transform targetPlayer)
    {
        player = targetPlayer;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        // Usar producto punto para saber qué tan atrás está respecto a la dirección del jugador
        float distanceForward = Vector3.Dot(transform.position - player.position, player.forward);

        if (distanceForward < -destroyBehindDistance)
        {
            Destroy(gameObject);
        }
    }
}
