using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sumfulla.Atomica
{
    public class NucleusLayer : MonoBehaviour
    {
        // References for particle system quantity limit
        // L1: Max = 2
        // L2: Max = 8
        // L3: Max = 20
        // L4: Max = 28
        // L5: Max = 50

        [SerializeField] private ParticleSystem _protons = null;
        [SerializeField] private ParticleSystem _neutrons = null;

        /// <summary>
        /// Configures arbitrary proton/neutron layers so distribution of larger quantities appears as a larger 'ball'
        /// </summary>
        public void UpdateNucleusLevel(ParticleType particleType, int particleCount)
        {
            ParticleSystem ps = particleType == ParticleType.Proton ? _protons : _neutrons;
            if (ps != null)
            {
                ParticleSystem.MainModule mainModule = ps.main;
                mainModule.maxParticles = particleCount;

                ParticleSystem.EmissionModule emissionModule = ps.emission;
                emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(particleCount, particleCount);

                ps.Simulate(0.0f, true, true);
                ps.Play();
            }
            else
            {
                Debug.LogError("Particle system is null");
            }
        }

        /// <summary>
        /// Enables/disables this nucleus layer is in use
        /// </summary>
        public void ActivateSystem(ParticleType particleType, bool activate)
        {
            ParticleSystem ps = particleType == ParticleType.Proton ? _protons : _neutrons;

            ps.gameObject.SetActive(activate);
        }
    }
}