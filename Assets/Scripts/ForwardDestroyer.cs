using UnityEngine;

public class ForwardDestroyer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float destroyBehindDistance = 25f;

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (transform.position.z < player.position.z - destroyBehindDistance)
        {
            Destroy(gameObject);
        }
    }
}
