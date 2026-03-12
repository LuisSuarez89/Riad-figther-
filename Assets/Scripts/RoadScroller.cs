using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    [SerializeField] private Renderer roadRenderer;
    [SerializeField] private float textureTilingSpeedMultiplier = 0.04f;

    private float textureOffset;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || roadRenderer == null)
        {
            return;
        }

        PlayerCarController player = FindObjectOfType<PlayerCarController>();
        if (player == null)
        {
            return;
        }

        textureOffset += player.CurrentForwardSpeed * textureTilingSpeedMultiplier * Time.deltaTime;
        roadRenderer.material.mainTextureOffset = new Vector2(0f, textureOffset);
    }
}
