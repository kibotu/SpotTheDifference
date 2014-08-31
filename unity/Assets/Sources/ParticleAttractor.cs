using UnityEngine;

namespace Assets.Sources
{
    public class ParticleAttractor : MonoBehaviour {

        public ParticleEmitter p;
        public Particle[] particles;
        public float affectDistance;
        float sqrDist;
        Transform thisTransform;

        void Start()
        {

            thisTransform = transform;
            particles = p.particles;
            sqrDist = affectDistance * affectDistance;
        }

        void Update()
        {
            if(p == null) 
                return;

            for (var i=0; i < particles.GetUpperBound(0);++i)
            {
                var dist = Vector3.SqrMagnitude(thisTransform.position - particles[i].position);
                if (dist < sqrDist) {
                    particles[i].position = Vector3.Lerp(particles[i].position,transform.position,Time.deltaTime / 2.0f);
                }
            }
            p.particles = particles;
        }
    }
}