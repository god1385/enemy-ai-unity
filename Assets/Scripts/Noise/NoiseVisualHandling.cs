using UnityEngine;

public class NoiseVisualHandling : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] CircleCollider2D _circleCollider;

    private float radius = 0f;
    private float strength;
    private ParticleSystem.Particle[] particles;

    public void SetRadius(float strength, float radius)
    {
        if (this.radius == 0f)
        {
            this.radius = radius;
            _circleCollider.radius = radius / 2;
        }

        this.strength = strength;
    }

    private void Update()
    {
        if (particles == null || particles.Length < _particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];

        int count = _particleSystem.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            float distanceFromCenter = particles[i].position.magnitude;

            if (distanceFromCenter > radius * (strength / 10f))
            {
                particles[i].startLifetime = 0f;
                particles[i].remainingLifetime = 0f;
            }
        }
        _particleSystem.SetParticles(particles, count);
    }
}
