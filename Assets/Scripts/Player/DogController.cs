using UnityEngine;

public enum DogState
{
    Walking,
    Sprinting,
    Standing,
    Sniffing,
    Carrying
}


public class DogController : MonoBehaviour
{
    public DogState CurrentState = DogState.Walking;

    private void Start()
    {
        Debug.Log("Dog Controller Initialized!");
    }
}