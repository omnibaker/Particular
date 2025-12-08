using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Sumfulla.Atomica
{
    public class EnergyLevel : MonoBehaviour
    {
        private const float ROT_FACTOR = 2f;

        [SerializeField] private ParticleSystem _s = null;
        [SerializeField] private ParticleSystem _p = null;
        [SerializeField] private ParticleSystem _d = null;
        [SerializeField] private ParticleSystem _f = null;

        private float _totalElectrons;
        private float _origSpeed;
        private float _currentRotationalShift;
        private float _rotationalFactor;

        /// <summary>
        /// Configures and sets up electron distribution for orbital particles
        /// </summary>
        public void UpdateElectrons(int s, int p, int d, int f)
        {
            _totalElectrons = s + p + d + f;
            _rotationalFactor = 360 / _totalElectrons;
            UpdateEnergyLevel("s", s);
            UpdateEnergyLevel("p", p);
            UpdateEnergyLevel("d", d);
            UpdateEnergyLevel("f", f);
        }


        /// <summary>
        /// Updates particles systems of specific orbitals on a given shell ring
        /// </summary>
        private void UpdateEnergyLevel(string orbital, int numOfElectrons)
        {
            ParticleSystem ps;
            switch (orbital)
            {
                case "s": ps = _s; break;
                case "p": ps = _p; break;
                case "d": ps = _d; break;
                case "f": ps = _f; break;
                default: ps = null; break;
            }

            // Add orbital instances
            if (ps != null)
            {
                ParticleSystem.MainModule mainModule = ps.main;
                mainModule.maxParticles = numOfElectrons;

                ParticleSystem.EmissionModule emissionModule = ps.emission;
                emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(_totalElectrons, _totalElectrons);

                ps.Simulate(0.0f, true, true);
                ps.Play();
            }
            else
            {
                Debug.LogError("Particle system is null");
            }

            // Add ringed positions
            ParticleSystem.ShapeModule shapeModule = ps.shape;
            shapeModule.rotation = new Vector3(0, 0, _currentRotationalShift);
            _currentRotationalShift += _rotationalFactor * numOfElectrons;

            // Add rotation
            ParticleSystem.VelocityOverLifetimeModule votModule = ps.velocityOverLifetime;
            if (_origSpeed == 0)
            {
                _origSpeed = votModule.orbitalZMultiplier;
            }
            votModule.orbitalZMultiplier = _origSpeed * ROT_FACTOR;
        }
    }
}