using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    [SerializeField] private float fuelAmount = 22f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.AddFuel(fuelAmount);
        Destroy(gameObject);
    }
}
