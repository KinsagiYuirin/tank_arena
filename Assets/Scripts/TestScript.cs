using System;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputReader.MoveEvent += HandleMove;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        _inputReader.MoveEvent -= HandleMove;
    }
    
    private void HandleMove(Vector2 movement)
    {
        Debug.Log(movement);
    }
}
