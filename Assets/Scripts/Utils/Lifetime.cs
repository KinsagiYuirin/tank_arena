using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
