using System.Collections.Generic;
using UnityEngine;

public class NoisePool : MonoBehaviour
{
    [SerializeField] private int poolSize = 5;
    [SerializeField] private Noise noisePrefab;

    private Queue<Noise> pool;

    private void Awake()
    {
        pool = new Queue<Noise>();

        for (int i = 0; i < poolSize; i++)
        {
            Noise tmpNoise = Instantiate(noisePrefab, transform);
            tmpNoise.gameObject.SetActive(false);
            tmpNoise.Initialize(this);
            pool.Enqueue(tmpNoise);
        }
    }

    public Noise GetNoise()
    {
        if (pool.Count > 0)
           return pool.Dequeue();

        Noise noise = Instantiate(noisePrefab, transform);
        noise.Initialize(this);
        pool.Enqueue(noise);

        return noise;
    }

    public void ReturnToPool(Noise noise)
    {
        pool.Enqueue(noise);
    }
}
