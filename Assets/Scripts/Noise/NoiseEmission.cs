using UnityEngine;

public class NoiseEmission : MonoBehaviour
{
    [SerializeField] private NoisePool pool;
    [SerializeField] private float noiseStrength;
    [SerializeField] private float coolDownBetweenNoises = 0.2f;

    private float timer = 0f;
    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    public void EmitNoise(Transform playerPosition)
    {
        if (timer > 0f)
            return;

        timer = coolDownBetweenNoises;

        Noise noise = pool.GetNoise();

        noise.ActivateNoise(playerPosition.position, noiseStrength);
    }
}
