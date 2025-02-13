using UnityEngine;

public abstract class Coin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected int coinValuel;
    protected bool alreadyCollected;

    public abstract int Collect();

    public void SetValue(int value)
    {
        coinValuel = value;
    }

    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }

// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
