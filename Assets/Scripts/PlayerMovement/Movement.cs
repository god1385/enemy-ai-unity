using Input;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private NoiseEmission noiseEmitter;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (InputHandler.MoveDirection.sqrMagnitude > 0)
            noiseEmitter.EmitNoise(transform);

        rb.MovePosition(rb.position + InputHandler.MoveDirection * Time.fixedDeltaTime * moveSpeed);
    }
}
