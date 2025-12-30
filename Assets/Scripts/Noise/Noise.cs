using UnityEngine;

public class Noise : MonoBehaviour
{
    [SerializeField] private NoiseVisualHandling visualHandling;

    private float noiseStrength;
    private float noiseRadius;
    private float noiseDecaySpeed = 1f;
    private bool isActivated = false;

    private NoisePool pool;

    public float NoiseStrength => noiseStrength;
    public float NoiseRadius => noiseRadius;

    public void Initialize(NoisePool pool)
    {
        this.pool = pool;
    }

    public void ActivateNoise(Vector2 position, float powerOfNoise)
    {
        noiseStrength = powerOfNoise;
        transform.position = position;
        noiseRadius = noiseStrength * 1.5f;
        gameObject.SetActive(true);
        isActivated = true;
    }

    private void Update()
    {
        if (isActivated)
        {
            noiseStrength -= noiseDecaySpeed * Time.deltaTime;

            if (visualHandling != null)
                visualHandling.SetRadius(noiseStrength, noiseRadius);

            if (noiseStrength < 0)
                Deactivate();
        }
    }

    public void Deactivate()
    {
        isActivated = false;
        gameObject.SetActive(false);
        pool.ReturnToPool(this);
    }
}
