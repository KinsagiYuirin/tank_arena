using System;
using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    [SerializeField] private TankPlayer player;
    [SerializeField] private SpriteRenderer[] playerSprite;
    [SerializeField] private Color[] tankColor;
    [SerializeField] private int colorIndex;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HandlePlayerColorChanged(0, player.PlayerColorIndex.Value);
    }

    private void HandlePlayerColorChanged(int oldIndex, int newIndex)
    {
        colorIndex = newIndex;
        foreach (SpriteRenderer sprite in playerSprite)
        {
            sprite.color = tankColor[colorIndex];
        }
    }

    private void OnDestroy()
    {
        player.PlayerColorIndex.OnValueChanged -= HandlePlayerColorChanged;
    }
}
