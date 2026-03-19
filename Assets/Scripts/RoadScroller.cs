using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    [SerializeField] private Renderer roadRenderer;
    [SerializeField] private float textureTilingSpeedMultiplier = 0.04f;

    private float textureOffset;
    private PlayerCarController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerCarController>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || roadRenderer == null)
        {
            return;
        }

        if (player == null)
        {
            player = FindObjectOfType<PlayerCarController>();
            if (player == null)
            {
                return;
            }
        }

        textureOffset += player.CurrentForwardSpeed * textureTilingSpeedMultiplier * Time.deltaTime;
        roadRenderer.material.mainTextureOffset = new Vector2(0f, textureOffset);
    }
}
